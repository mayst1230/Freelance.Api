using Freelance.Api.Exceptions;
using Freelance.Api.Extensions;
using Freelance.Api.Interfaces;
using Freelance.Api.v1.ReferenceItems;
using Freelance.Core.Models;
using Freelance.Core.Models.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Orders;

/// <summary>
/// Работа с заказами.
/// </summary>
[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class OrdersController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IEmailSender _emailSender;

    public OrdersController(
        DataContext dataContext,
        IEmailSender emailSender)
    {
        _dataContext = dataContext;
        _emailSender = emailSender;
    }

    /// <summary>
    /// Получение списка заказов услуг на исполнение (исполнитель).
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Список заказов услуг на исполнение.</returns>
    [HttpGet("list/execution")]
    [Authorize(Roles = "Contractor")]
    public async Task<OrderListResponse> ExecutionListAsync([FromQuery] OrderListExecutionRequest request)
    {
        var list = _dataContext.Orders.Include(i => i.Customer)
                                       .Where(i => !i.IsDeleted
                                                    && i.Status == OrderStatus.Execution 
                                                    && (!i.Customer.IsDeleted || i.Customer.IsBanned))
                                       .Select(i => new OrderItem()
        {
            UniqueIdentifier = i.UniqueIdentifier,
            Title = i.Title,
            Description = i.Description,
            Price = i.Price,
            Status = i.Status,
            Started = i.Started,
            Canceled = i.Canceled,
            Created = i.Created,
            Updated = i.Updated,
            Customer = new UserReferenceItem()
            {
                UniqueIdentifier = i.Customer.UniqueIdentifier,
                UserName = i.Customer.UserName,
                Rating = i.Customer.Rating,
                PhotoFile = i.Customer.PhotoFile != null ? new FileReferenceItem()
                {
                    UniqueIdentifier = i.Customer.PhotoFile.UniqueIdentifier,
                    FileName = i.Customer.PhotoFile.FileName,
                    DisplayName = i.Customer.PhotoFile.DisplayName,
                    GroupType = i.Customer.PhotoFile.GroupType,
                    MimeType = i.Customer.PhotoFile.MimeType,
                    Size = i.Customer.PhotoFile.Size,
                    Url = Url.Action("Download", "Files", new { fileUuid = i.Customer.PhotoFile.UniqueIdentifier })!
                } : default,
            },
        });

        return new OrderListResponse()
        {
            TotalCount = await list.CountAsync(),
            Items = await list.OrderBy(i => i.Updated)
                              .LimitOffset(request.Limit, request.Offset)
                              .ToArrayAsync()
        };
    }

    /// <summary>
    /// Получение списка собственных заказов услуг.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Список собственных заказов услуг.</returns>
    [HttpGet("list/me")]
    [Authorize(Roles = "Customer, Contractor")]
    public async Task<OrderListResponse> MeListAsync([FromQuery] OrderMeListRequest request)
    {
        var userId = User.GetUserId() ?? throw new ApiForbidException("Пользователь не определен.");

        var list = _dataContext.Orders.Where(i => request.IsDeleted && (i.ContractorId == userId || i.CustomerId == userId))
                                      .Include(i => i.Customer)
                                      .Include(i => i.Contractor)
                                      .Select(i => new OrderItem()
                                      {
                                          UniqueIdentifier = i.UniqueIdentifier,
                                          Title = i.Title,
                                          Description = i.Description,
                                          Price = i.Price,
                                          Status = i.Status,
                                          Started = i.Started,
                                          Canceled = i.Canceled,
                                          Created = i.Created,
                                          Updated = i.Updated,
                                          Customer = new UserReferenceItem()
                                          {
                                              UniqueIdentifier = i.Customer.UniqueIdentifier,
                                              UserName = i.Customer.UserName,
                                              Rating = i.Customer.Rating,
                                              PhotoFile = i.Customer.PhotoFile != null ? new FileReferenceItem()
                                              {
                                                  UniqueIdentifier = i.Customer.PhotoFile.UniqueIdentifier,
                                                  FileName = i.Customer.PhotoFile.FileName,
                                                  DisplayName = i.Customer.PhotoFile.DisplayName,
                                                  GroupType = i.Customer.PhotoFile.GroupType,
                                                  MimeType = i.Customer.PhotoFile.MimeType,
                                                  Size = i.Customer.PhotoFile.Size,
                                                  Url = Url.Action("Download", "Files", new { fileUuid = i.Customer.PhotoFile.UniqueIdentifier })!
                                              } : default,
                                          },
                                          Contractor = i.Contractor != null ? new UserReferenceItem()
                                          {
                                              UniqueIdentifier = i.Contractor.UniqueIdentifier,
                                              UserName = i.Contractor.UserName,
                                              Rating = i.Contractor.Rating,
                                              PhotoFile = i.Contractor.PhotoFile != null ? new FileReferenceItem()
                                              {
                                                  UniqueIdentifier = i.Contractor.PhotoFile.UniqueIdentifier,
                                                  FileName = i.Contractor.PhotoFile.FileName,
                                                  DisplayName = i.Contractor.PhotoFile.DisplayName,
                                                  MimeType = i.Contractor.PhotoFile.MimeType,
                                                  GroupType = i.Contractor.PhotoFile.GroupType,
                                                  Url = Url.Action("Download", "Files", new { fileUuid = i.Contractor.PhotoFile.UniqueIdentifier })!
                                              } : default,
                                          } : default,
                                      });

        if (request.Status.HasValue)
            list = list.Where(i => i.Status == request.Status.Value);

        return new OrderListResponse()
        {
            TotalCount = await list.CountAsync(),
            Items = await list.OrderBy(i => i.Updated)
                              .LimitOffset(request.Limit, request.Offset)
                              .ToArrayAsync()
        };
    }

    /// <summary>
    /// Получение сведений о заказе услуги.
    /// </summary>
    /// <param name="orderUuid">УИД заказа.</param>
    /// <returns>Сведения о заказе услуги.</returns>
    [HttpGet("details")]
    [Authorize(Roles = "Customer, Contractor")]
    public async Task<OrderItem> DetailsAsync([FromQuery][Required] Guid orderUuid)
    {
        var order = await _dataContext.Orders.Where(i => !i.IsDeleted && i.UniqueIdentifier == orderUuid).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Заказ услуги не найден.");

        return await GetDbItemAsync(order);
    }

    /// <summary>
    /// Создание заказа услуги.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения о созданном заказе.</returns>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<OrderItem> CreateAsync([FromQuery] OrderCreateRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new ApiNotFoundException("Пользователь не определен.");
        var user = await _dataContext.Users.Where(i => !i.IsDeleted && !i.IsBanned && i.Id == userId)
                                           .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

        var order = new Order()
        {
            CustomerId = userId,
            CustomerFileId = request.CustomerFileId,
            Price = request.Price,
            Title = request.Title,
            Description = request.Description,
            Status = OrderStatus.Execution,
            Started = request.Started,
            Canceled = request.Canceled,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };

        await _dataContext.Orders.AddAsync(order);
        await _dataContext.SaveChangesAsync();

        return await GetDbItemAsync(order);
    }

    /// <summary>
    /// Редактирование заказа услуги.
    /// </summary>
    /// <param name="orderUuid">УИД заказа услуги</param>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения о измененном заказе услуги.</returns>
    [HttpPut]
    [Authorize(Roles = "Customer, Contractor")]
    public async Task<OrderItem> EditAsync([FromQuery][Required] Guid orderUuid, [FromBody] OrderEditRequest request)
    {
        var order = await _dataContext.Orders.Where(i => i.IsDeleted && i.UniqueIdentifier == orderUuid).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Заказ услуги не найден.");

        if (order.CustomerId == User.GetUserId() && request.CustomerFileId.HasValue)
            order.CustomerFileId = request.CustomerFileId.Value;

        if (order.ContractorId == User.GetUserId() && request.ContractorFileId.HasValue)
            order.ContractorFileId = request.ContractorFileId.Value;

        order.Price = request.Price;
        order.Title = request.Title;
        order.Description = request.Description;
        order.Started = request.Started;
        order.Canceled = request.Canceled;
        order.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        return await GetDbItemAsync(order);
    }

    /// <summary>
    /// Изменение статуса заказа услуги.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения о заказе услуги.</returns>
    [HttpPut("status")]
    [Authorize(Roles = "Customer, Contractor")]
    public async Task<OrderItem> ChangeStatusAsync([FromQuery][Required] OrderChangeStatusRequest request)
    {
        var order = await _dataContext.Orders
                                      .Include(i => i.Contractor)
                                      .Include(i => i.Customer)
                                      .Where(i => i.IsDeleted && i.UniqueIdentifier == request.UniqueIdentifier)
                                      .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Заказ услуги не найден.");

        if (request.Status == OrderStatus.Made)
        {
            var customerBalance = await _dataContext.UserBalances.Where(i => i.UserId == order.CustomerId).FirstOrDefaultAsync()
                ?? throw new ApiNotFoundException("Счет заказчика не найден.");

            var contractorBalance = await _dataContext.UserBalances.Where(i => i.UserId == order.ContractorId).FirstOrDefaultAsync()
                ?? throw new ApiNotFoundException("Счет исполнителя не найден.");

            using var tr = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var customerBalanceLog = new UserBalanceLog()
                {
                    UserId = customerBalance.UserId,
                    BalanceBefore = customerBalance.Balance,
                    Type = TypeGroup.Payment,
                };

                customerBalance.Balance -= order.Price;
                customerBalance.Updated = DateTimeOffset.UtcNow;
                await _dataContext.SaveChangesAsync();

                customerBalanceLog.BalanceAfter = customerBalance.Balance;
                customerBalanceLog.Credit = order.Price;
                customerBalanceLog.Debit = 0.0m;
                customerBalanceLog.Created = DateTimeOffset.UtcNow;

                await _dataContext.UserBalanceLogs.AddAsync(customerBalanceLog);
                await _dataContext.SaveChangesAsync();

                var contractorBalanceLog = new UserBalanceLog()
                {
                    UserId = contractorBalance.UserId,
                    BalanceBefore = contractorBalance.Balance,
                    Type = TypeGroup.Receiving,
                };

                contractorBalance.Balance += order.Price;
                contractorBalance.Updated = DateTimeOffset.UtcNow;
                await _dataContext.SaveChangesAsync();

                contractorBalanceLog.BalanceAfter = contractorBalance.Balance;
                contractorBalanceLog.Credit = 0.0m;
                contractorBalanceLog.Debit = order.Price;
                contractorBalanceLog.Created = DateTimeOffset.UtcNow;

                await _dataContext.UserBalanceLogs.AddAsync(contractorBalanceLog);
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                await tr.RollbackAsync();
                throw new ApiException("Ошибка при переводе статуса заказа услуги в статус 'Сделан'.");
            }

            await tr.CommitAsync();
        }

        order.Status = request.Status;
        order.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        if (order.Contractor != null)
            _emailSender.SendEmailStatusOrder(order.Contractor.Email, order);

        _emailSender.SendEmailStatusOrder(order.Customer.Email, order);

        return await GetDbItemAsync(order);
    }

    /// <summary>
    /// Список доступых действий над заказом услуги.
    /// </summary>
    /// <param name="orderUuid">УИД заказа услуги.</param>
    /// <returns>Список доступых действий над заказом услуги.</returns>
    [HttpGet("actions")]
    [Authorize(Roles = "Customer, Contractor")]
    public async Task<List<OrderStatus>> ActionsAsync([FromQuery][Required] Guid orderUuid)
    {
        var userId = User.GetUserId() ?? throw new ApiForbidException("Пользователь не определен.");
        var userRole = User.GetUserRole();

        var order = await _dataContext.Orders.Where(i => !i.IsDeleted && i.UniqueIdentifier == orderUuid).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Заказ услуги не найден.");

        var actionsList = new List<OrderStatus>();

        if (userRole == UserRole.Customer.ToString())
        {
            if (order.Status == OrderStatus.Review)
            {
                actionsList.Add(OrderStatus.Rework);
                actionsList.Add(OrderStatus.Made);
            }
        }
        else
        {
            if (order.Status == OrderStatus.Execution || order.Status == OrderStatus.Rework)
                actionsList.Add(OrderStatus.Development);
            else if (order.Status == OrderStatus.Development)
                actionsList.Add(OrderStatus.Review);
        }

        return actionsList;
    }

    /// <summary>
    /// Сведения о заказе услуги.
    /// </summary>
    /// <param name="order">Заказ услуги.</param>
    /// <returns>Сведения о заказе услуги.</returns>
    private async Task<OrderItem> GetDbItemAsync(Order order)
    {
        var customer = await _dataContext.Users.Where(i => i.Id == order.CustomerId).Select(i => new
        {
            i.UniqueIdentifier,
            i.UserName,
            i.Rating,
            i.PhotoFile,
        }).FirstOrDefaultAsync() ?? throw new ApiException("Заказчик в заказе услуги не найден.");

        var customerFile = await _dataContext.Files.Where(i => i.Id == order.CustomerFileId).FirstOrDefaultAsync();

        var contractor = await _dataContext.Users.Where(i => i.Id == order.ContractorId).Select(i => new
        {
            i.UniqueIdentifier,
            i.UserName,
            i.Rating,
            i.PhotoFile,
        }).FirstOrDefaultAsync();

        var contractorFile = await _dataContext.Files.Where(i => i.Id == order.ContractorFileId).FirstOrDefaultAsync();

        return new OrderItem()
        {
            UniqueIdentifier = order.UniqueIdentifier,
            Title = order.Title,
            Description = order.Description,
            Price = order.Price,
            Status = order.Status,
            Started = order.Started,
            Canceled = order.Canceled,
            IsDeleted = order.IsDeleted,
            Created = order.Created,
            Updated = order.Updated,
            Customer = new UserReferenceItem()
            {
                UniqueIdentifier = customer.UniqueIdentifier,
                UserName = customer.UserName,
                Rating = customer.Rating,
                PhotoFile = customer.PhotoFile != null ? new FileReferenceItem()
                {
                    UniqueIdentifier = customer.PhotoFile.UniqueIdentifier,
                    FileName = customer.PhotoFile.FileName,
                    DisplayName = customer.PhotoFile.DisplayName,
                    MimeType = customer.PhotoFile.MimeType,
                    GroupType = customer.PhotoFile.GroupType,
                    Url = Url.Action("Download", "Files", new { fileUuid = customer.PhotoFile.UniqueIdentifier })!
                } : default,
            },
            CustomerFile = customerFile != null ? new FileReferenceItem()
            {
                UniqueIdentifier = customerFile.UniqueIdentifier,
                FileName = customerFile.FileName,
                DisplayName = customerFile.DisplayName,
                MimeType = customerFile.MimeType,
                Size = customerFile.Size,
                GroupType = customerFile.GroupType,
                Url = Url.Action("Download", "Files", new { fileUuid = customerFile.UniqueIdentifier })!
            } : default,
            Contractor = contractor != null ? new UserReferenceItem()
            {
                UniqueIdentifier = contractor.UniqueIdentifier,
                UserName = contractor.UserName,
                Rating = contractor.Rating,
                PhotoFile = contractor.PhotoFile != null ? new FileReferenceItem()
                {
                    UniqueIdentifier = contractor.PhotoFile.UniqueIdentifier,
                    FileName = contractor.PhotoFile.FileName,
                    DisplayName = contractor.PhotoFile.DisplayName,
                    MimeType = contractor.PhotoFile.MimeType,
                    GroupType = contractor.PhotoFile.GroupType,
                    Url = Url.Action("Download", "Files", new { fileUuid = contractor.PhotoFile.UniqueIdentifier })!
                } : default,
            } : default,
            ContractorFile = contractorFile != null ? new FileReferenceItem()
            {
                UniqueIdentifier = contractorFile.UniqueIdentifier,
                FileName = contractorFile.FileName,
                DisplayName = contractorFile.DisplayName,
                MimeType = contractorFile.MimeType,
                Size = contractorFile.Size,
                GroupType = contractorFile.GroupType,
                Url = Url.Action("Download", "Files", new { fileUuid = contractorFile.UniqueIdentifier })!
            } : default,
        };
    }
}
