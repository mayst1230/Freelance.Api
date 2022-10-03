using Freelance.Api.Exceptions;
using Freelance.Api.Extensions;
using Freelance.Api.Interfaces;
using Freelance.Core.Models;
using Freelance.Core.Models.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Freelance.Api.v1.UserBalances;

/// <summary>
/// Работа со счетами пользователей.
/// </summary>
[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class UserBalancesController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IPaymentService _paymentService;

    public UserBalancesController(
        DataContext dataContext,
        IPaymentService paymentService)
    {
        _dataContext = dataContext;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Получение списка операций текущего пользователя (заказчика, исполнителя).
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Список операций со счетом текущего пользователя.</returns>
    [HttpGet("me/operations")]
    [Authorize(Roles = "Customer, Contractor")]
    public async Task<UserBalanceLogListResponse> LogsListAsync([FromQuery] UserBalanceLogListRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new InvalidOperationException();

        var list = _dataContext.UserBalanceLogs.Where(i => i.UserId == userId)
                                               .Select(i => new UserBalanceLogItem()
                                               {
                                                   UniqueIdentifier = i.UniqueIdentifier,
                                                   BalanceBefore = i.BalanceBefore,
                                                   BalanceAfter = i.BalanceAfter,
                                                   Debit = i.Debit,
                                                   Credit = i.Credit,
                                                   Type = i.Type,
                                                   Created = i.Created,
                                               });

        if (request.Type.HasValue)
            list = list.Where(i => i.Type == request.Type.Value);

        return new UserBalanceLogListResponse()
        {
            TotalCount = await list.CountAsync(),
            Items = await list.OrderByDescending(i => i.Created)
                              .LimitOffset(request.Limit, request.Offset)
                              .ToArrayAsync()
        };
    }

    /// <summary>
    /// Пополнение собственного счета (заказчика).
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Информация о собственном счете (заказчика).</returns>
    [HttpPost("me/replenishment")]
    [Authorize(Roles = "Customer")]
    public async Task<UserBalanceOperationResponse> ReplenishmentAsync([FromQuery] UserBalanceReplenishmentRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new InvalidOperationException();
        var userUuid = User.GetUserUuid() ?? throw new InvalidOperationException();

        var userBalance = await _dataContext.UserBalances.Where(i => i.UserId == userId)
                                                         .FirstOrDefaultAsync() ?? throw new ApiException("Счет пользователя не найден.");

        if (!_paymentService.Replenishment(request.CardNumber, request.Cvc, request.AmountReplenishment))
            throw new ApiException("Ошибка в платежной системе при пополнеии счета.");

        var result = new UserBalanceOperationResponse()
        {
            UserUniqueIdentifier = userUuid,
            OperationType = TypeGroup.Replenishment,
            BalanceBefore = userBalance.Balance,
        };

        using var tr = await _dataContext.Database.BeginTransactionAsync();
        try
        {
            var userBalanceLog = new UserBalanceLog()
            {
                UserId = userBalance.UserId,
                BalanceBefore = userBalance.Balance,
                Type = TypeGroup.Replenishment,
            };

            userBalance.Balance += request.AmountReplenishment;
            userBalance.Updated = DateTimeOffset.UtcNow;

            _dataContext.UserBalances.Update(userBalance);
            await _dataContext.SaveChangesAsync();

            userBalanceLog.BalanceAfter = userBalance.Balance;
            userBalanceLog.Credit = 0.0m;
            userBalanceLog.Debit = request.AmountReplenishment;
            userBalanceLog.Created = DateTimeOffset.UtcNow;

            await _dataContext.UserBalanceLogs.AddAsync(userBalanceLog);
            await _dataContext.SaveChangesAsync();

            await tr.CommitAsync();

            result.BalanceAfter = userBalance.Balance;
            return result;
        }
        catch
        {
            await tr.RollbackAsync();
            throw new ApiException("Ошибка при пополнении счета пользователя.");
        }
    }

    /// <summary>
    /// Вывод средств со собственного счета (заказчика, исполнителя).
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Информация о собственном счете (заказачика, исполнителя).</returns>
    [HttpPost("me/withdrawal")]
    [Authorize(Roles = "Customer, Contractor")]
    public async Task<UserBalanceOperationResponse> WithdrawalAsync([FromQuery] UserBalanceWithdrawalRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new InvalidOperationException();
        var userUuid = User.GetUserUuid() ?? throw new InvalidOperationException();

        var userBalance = await _dataContext.UserBalances.Where(i => i.UserId == userId)
                                                         .FirstOrDefaultAsync() ?? throw new ApiException("Счет пользователя не найден.");

        if (!_paymentService.Withdrawal(request.CardNumber, request.AmountWithdrawal))
            throw new ApiException("Ошибка в платежной системе при выводе средств.");

        if (userBalance.Balance - request.AmountWithdrawal < 0.00m)
            throw new ApiException("Сумма средств для вывода превышает баланс пользователя.");

        var result = new UserBalanceOperationResponse()
        {
            UserUniqueIdentifier = userUuid,
            OperationType = TypeGroup.Withdrawal,
            BalanceBefore = userBalance.Balance,
        };

        using var tr = await _dataContext.Database.BeginTransactionAsync();
        try
        {
            var userBalanceLog = new UserBalanceLog()
            {
                UserId = userBalance.UserId,
                BalanceBefore = userBalance.Balance,
                Type = TypeGroup.Withdrawal,
            };

            userBalance.Balance -= request.AmountWithdrawal;
            userBalance.Updated = DateTimeOffset.UtcNow;

            _dataContext.UserBalances.Update(userBalance);
            await _dataContext.SaveChangesAsync();

            userBalanceLog.BalanceAfter = userBalance.Balance;
            userBalanceLog.Credit = request.AmountWithdrawal;
            userBalanceLog.Debit = 0.0m;
            userBalanceLog.Created = DateTimeOffset.UtcNow;

            await _dataContext.UserBalanceLogs.AddAsync(userBalanceLog);
            await _dataContext.SaveChangesAsync();

            await tr.CommitAsync();

            result.BalanceAfter = userBalance.Balance;
            return result;
        }
        catch
        {
            await tr.RollbackAsync();
            throw new ApiException("Ошибка при выводе средств со счета пользователя.");
        }
    }
}
