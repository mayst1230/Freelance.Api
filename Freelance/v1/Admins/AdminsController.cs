using Freelance.Api.Exceptions;
using Freelance.Api.Interfaces;
using Freelance.Api.v1.Users;
using Freelance.Core.Models;
using Freelance.Core.Models.Storage;
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
    public class AdminsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;
        private readonly IJwtHandler _jwtHandler;

        public AdminsController(
            DataContext dataContext,
            IUserService userService,
            IJwtHandler jwtHandler)
        {
            _dataContext = dataContext;
            _userService = userService;
            _jwtHandler = jwtHandler;
        }

        /// <summary>
        /// Получение фейкового токена доступа для тестирования системы.
        /// </summary>
        /// <returns>Фейковый токен доступа.</returns>
        [HttpGet("tokens/fake")]
        public string GetFakeAccessToken()
        {
            return _jwtHandler.GenerateToken(new User()
            {
                Id = 10,
                UniqueIdentifier = Guid.Parse("7935ccc5-6b4a-4d5f-a8ef-cb2e4e404598"),
                UserName = "fakeUser",
                FirstName = "fakeUser",
                LastName = "fakeUser",
                MiddleName = "fakeUser",
                Email = "fakeUserEmail",
                Password = "fakeUser",
                Role = UserRole.Admin,
                Rating = 0m,
                Created = DateTimeOffset.Now,
                Updated = DateTimeOffset.Now,
            });
        }

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные об удаленном пользователе.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("users/delete")]
        public async Task<UserItem> DeleteAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => !i.IsDeleted && i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            user.IsDeleted = true;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }

        /// <summary>
        /// Восстановление пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные об восстановленном пользователе.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("users/restore")]
        public async Task<UserItem> RestoreAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => i.IsDeleted && i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            user.IsDeleted = false;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }

        /// <summary>
        /// Блокировка пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные о заблокированном пользователе.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("users/block")]
        public async Task<UserItem> BlockAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => !i.IsBanned && i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            user.IsBanned = true;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }

        /// <summary>
        /// Разблокировка пользователя.
        /// </summary>
        /// <param name="userUuid">УИД пользователя.</param>
        /// <returns>Данные о разблокированном пользователе.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("users/unblock")]
        public async Task<UserItem> UnblockAsync([FromQuery][Required] Guid userUuid)
        {
            if (!ModelState.IsValid)
                throw new ApiException();

            var user = await _dataContext.Users.Where(i => i.IsBanned && i.UniqueIdentifier == userUuid)
                                               .FirstOrDefaultAsync() ?? throw new ApiNotFoundException("Пользователь не найден.");

            user.IsBanned = false;
            user.Updated = DateTimeOffset.UtcNow;

            await _dataContext.SaveChangesAsync();

            return _userService.GetUserInfo(user);
        }
    }
}
