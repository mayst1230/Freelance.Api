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

namespace Freelance.Api.v1.Users;

/// <summary>
/// Работа с пользователями.
/// </summary>
[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IUserService _userService;
    private readonly IEmailSender _emailSender;

    public UsersController(
        DataContext dataContext,
        IUserService userService,
        IEmailSender emailSender)
    {
        _dataContext = dataContext;
        _userService = userService;
        _emailSender = emailSender;
    }

    /// <summary>
    /// Список пользователей.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Список пользователей.</returns>
    [HttpGet]
    [Authorize]
    public async Task<UserListResponse> ListAsync([FromQuery] UserListRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var list = _dataContext.Users.Where(i => !i.IsBanned && !i.IsDeleted).Select(i => new UserItem()
        {
            UniqueIdentifier = i.UniqueIdentifier,
            UserName = i.UserName,
            FirstName = i.FirstName,
            LastName = i.LastName,
            MiddleName = i.MiddleName,
            Role = i.Role,
            Email = i.Email,
            Rating = i.Rating,
            PhotoFile = i.PhotoFile != default ? new FileReferenceItem()
            {
                UniqueIdentifier = i.PhotoFile.UniqueIdentifier,
                FileName = i.PhotoFile.FileName,
                DisplayName = i.PhotoFile.DisplayName,
                GroupType = i.PhotoFile.GroupType,
                MimeType = i.PhotoFile.MimeType,
                Size = i.PhotoFile.Size,
                Url = Url.Action("Download", "Files", new { fileUuid = i.PhotoFile.UniqueIdentifier })!
            } : default,
            IsDeleted = i.IsDeleted,
            IsBanned = i.IsBanned,
            Created = i.Created,
            Updated = i.Updated,
        });

        if (request.UserUniqueIdentifier.HasValue)
            list = list.Where(i => i.UniqueIdentifier == request.UserUniqueIdentifier.Value);

        if (request.Role.HasValue)
            list = list.Where(i => i.Role == request.Role.Value);

        if (request.Rating.HasValue)
            list = list.Where(i => i.Rating == request.Rating.Value);

        if (request.IsDeleted.HasValue)
            list = list.Where(i => i.IsDeleted == request.IsDeleted.Value);

        if (request.IsBanned.HasValue)
            list = list.Where(i => i.IsBanned == request.IsBanned.Value);

        if (!string.IsNullOrEmpty(request.UserName))
            list = list.Where(i => i.UserName == request.UserName);

        if (!string.IsNullOrEmpty(request.FirstName))
            list = list.Where(i => i.FirstName == request.FirstName);

        if (!string.IsNullOrEmpty(request.LastName))
            list = list.Where(i => i.LastName == request.LastName);

        if (!string.IsNullOrEmpty(request.MiddleName))
            list = list.Where(i => i.MiddleName == request.MiddleName);

        if (!string.IsNullOrEmpty(request.Email))
            list = list.Where(i => i.Email == request.Email);

        return new UserListResponse()
        {
            TotalCount = await list.CountAsync(),
            Items = await list.OrderBy(i => i.UserName)
                              .LimitOffset(request.Limit, request.Offset)
                              .ToArrayAsync()
        };
    }

    /// <summary>
    /// Регистрация пользователя.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Данные о зарегистрированном пользователе.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<UserItem> RegisterAsync([FromQuery] UserRegisterRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        if (await _dataContext.Users.AnyAsync(i => !i.IsDeleted && i.IsBanned && (i.UserName == request.UserName || i.Email == request.Email)))
            throw new ApiException("Пользователь уже существует.");

        using var tr = await _dataContext.Database.BeginTransactionAsync();

        var user = new User()
        {
            UserName = request.UserName,
            Role = request.Role,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhotoFileId = request.PhotoFileId,
            Rating = 0.0m,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };

        await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();

        await tr.CommitAsync();

        if (!await CreateUserBalanceAsync(user.Id))
        {
            await tr.RollbackAsync();
            throw new ApiException("Ошибка при создании счёта пользователя.");
        }    

        return _userService.GetUserInfo(user, true);
    }

    /// <summary>
    /// Авторизация пользователя.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Данные об авторизованном пользователе.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<UserItem> LoginAsync([FromQuery] UserLoginRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var user = await _dataContext.Users.Where(i => !i.IsDeleted && !i.IsBanned && i.UserName == request.UserName)
                                           .FirstOrDefaultAsync() ?? throw new ApiException("Неверный логин.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new ApiException("Неверный пароль.");

        return _userService.GetUserInfo(user, true);
    }

    /// <summary>
    /// Получение сведений о пользователе.
    /// </summary>
    /// <param name="userUuid">УИД пользователя.</param>
    /// <returns>Сведения о пользователе.</returns>
    [HttpGet("info")]
    [Authorize]
    public UserItem Details([FromQuery][Required] Guid userUuid)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var item = (
            from u in _dataContext.Users
            join f in _dataContext.Files on u.PhotoFileId equals f.Id into fG
            from f in fG.DefaultIfEmpty()
            where !u.IsBanned && !u.IsDeleted && u.UniqueIdentifier == userUuid
            select new
            {
                User = u,
                File = f,
            }
        ).FirstOrDefault() ?? throw new ApiNotFoundException("Пользователь не найден.");

        item.User.PhotoFile = item.File;
        return _userService.GetUserInfo(item.User);
    }


    /// <summary>
    /// Редактирование собственного профиля.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения об измененном пользователе.</returns>
    [HttpPut("me/edit")]
    [Authorize]
    public async Task<UserItem> EditAsync([FromBody] UserEditRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userUuid = User.GetUserUuid() ?? throw new InvalidOperationException();

        var item = (
            from u in _dataContext.Users
            join f in _dataContext.Files on u.PhotoFileId equals f.Id into fG
            from f in fG.DefaultIfEmpty()
            where  !u.IsBanned && !u.IsDeleted && u.UniqueIdentifier == userUuid
            select new 
            { 
                User = u,
                File = f,
            }
        ).FirstOrDefault() ?? throw new ApiNotFoundException("Пользователь не найден.");

        item.User.UserName = request.UserName;
        item.User.FirstName = request.FirstName;
        item.User.LastName = request.LastName;
        item.User.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        item.User.MiddleName = request.MiddleName;
        item.User.Email = request.Email;
        item.User.PhotoFileId = request.PhotoFileId;
        item.User.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        item.User.PhotoFile = item.File;
        return _userService.GetUserInfo(item.User);
    }

    /// <summary>
    /// Отправка кода восстановления для пароля на почту.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Результат отправки кода восстановления на почту.</returns>
    [HttpPut("send/code")]
    [AllowAnonymous]
    public async Task<UserRestorePasswordResponse> SendEmailRestoreCodeAsync([FromQuery][Required] string email)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var user = await _dataContext.Users.Where(i => !i.IsDeleted && !i.IsBanned && i.Email == email).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Пользователь не найден.");

        var restoreCode = new Random().Next(100000, 999999).ToString() 
            ?? throw new ApiException("Ошибка при генерации кода восстановления.");

        user.RestorePasswordCode = BCrypt.Net.BCrypt.HashPassword(restoreCode);
        user.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        var sendEmailResult = _emailSender.SendEmailRestorePasswordCode(email, restoreCode);

        return new UserRestorePasswordResponse() 
        { 
            UserUniqueIdentifier = user.UniqueIdentifier,
            IsSuccess = sendEmailResult,
        };
    }

    /// <summary>
    /// Восстановление пароля.
    /// </summary>
    /// <returns></returns>
    [HttpPut("restore/password")]
    [AllowAnonymous]
    public async Task<UserRestorePasswordResponse> RestorePasswordAsync([FromQuery] UserRestorePasswordRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var user = await _dataContext.Users.Where(i => !i.IsBanned && !i.IsDeleted && i.UniqueIdentifier == request.UserUniqueIdentifier).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Пользователь не найден.");

        if (!BCrypt.Net.BCrypt.Verify(request.Code, user.RestorePasswordCode))
            throw new ApiException("Неверный код для восстановления пароля.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.RestorePasswordCode = default;
        user.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        return new UserRestorePasswordResponse() 
        { 
            UserUniqueIdentifier = user.UniqueIdentifier,
            IsSuccess = true,
        };
    }

    /// <summary>
    /// Создание счёта пользователя.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <returns>Результат создания счёта пользователя.</returns>
    private async Task<bool> CreateUserBalanceAsync(int userId)
    {
        try
        {
            var userBalance = new UserBalance()
            {
                UserId = userId,
                Balance = 0.00m,
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
            };

            await _dataContext.UserBalances.AddAsync(userBalance);
            await _dataContext.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
