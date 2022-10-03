using Freelance.Api.Exceptions;
using Freelance.Api.Interfaces;
using Freelance.Api.v1.Users;
using Freelance.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Admins
{
    /// <summary>
    /// Работа с администраторами.
    /// </summary>
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;

        public AdminsController(
            DataContext dataContext,
            IUserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
        }

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные об удаленном пользователе.</returns>
        [HttpDelete("users/delete")]
        public async Task<UserItem> DeleteAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            if (user.IsDeleted)
                throw new ApiException("Пользователь уже удален.");

            user.IsDeleted = true;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }

        /// <summary>
        /// Восстановление пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные об восстановленном пользователе.</returns>
        [HttpPut("users/restore")]
        public async Task<UserItem> RestoreAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            if (!user.IsDeleted)
                throw new ApiException("Пользователь не удален.");

            user.IsDeleted = false;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }

        /// <summary>
        /// Блокировка пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные о заблокированном пользователе.</returns>
        [HttpPut("users/block")]
        public async Task<UserItem> BlockAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            if (user.IsBanned)
                throw new ApiException("Пользователь уже заблокирован.");

            user.IsBanned = true;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }

        /// <summary>
        /// Разблокировка пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные о разблокированном пользователе.</returns>
        [HttpPut("users/unblock")]
        public async Task<UserItem> UnblockAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            if (!user.IsBanned)
                throw new ApiException("Пользователь не заблокирован.");

            user.IsBanned = false;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }
    }
}
