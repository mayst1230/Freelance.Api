using Freelance.Api.Interfaces;
using Freelance.Api.v1.ReferenceItems;
using Freelance.Api.v1.Users;
using Freelance.Core.Models.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Freelance.Api.Services
{
    /// <summary>
    /// Служба для работы с пользователями.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IJwtHandler _jwtHandler;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public UserService(
            IJwtHandler jwtHandler,
            IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory)
        { 
            _jwtHandler = jwtHandler;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        /// <summary>
        /// Сведения о пользователе.
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="generateAccessToken">Сгенрировать токен доступа.</param>
        /// <returns>Сведения о пользователе.</returns>
        public UserItem GetUserInfo(User user, bool generateAccessToken = false)
        {
            return new UserItem()
            {
                UniqueIdentifier = user.UniqueIdentifier,
                UserName = user.UserName,
                Role = user.Role,
                Email = user.Email,
                Rating = user.Rating,
                PhotoFile = user.PhotoFile != default ? new FileReferenceItem()
                {
                    UniqueIdentifier = user.PhotoFile.UniqueIdentifier,
                    FileName = user.PhotoFile.FileName,
                    DisplayName = user.PhotoFile.DisplayName,
                    GroupType = user.PhotoFile.GroupType,
                    MimeType = user.PhotoFile.MimeType,
                    Size = user.PhotoFile.Size,
                    Url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext!).Action("Download", "Files", new { fileUuid = user.PhotoFile.UniqueIdentifier })!
                } : default,
                IsDeleted = user.IsDeleted,
                IsBanned = user.IsBanned,
                Created = user.Created,
                Updated = user.Updated,
                AccessToken = generateAccessToken ? _jwtHandler.GenerateToken(user) : default,
            };
        }
    }
}
