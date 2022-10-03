using Freelance.Api.v1.Users;
using Freelance.Core.Models.Storage;

namespace Freelance.Api.Interfaces
{
    /// <summary>
    /// Пользователи.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Сведения о пользователе.
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="generateAccessToken">Сгенрировать токен доступа.</param>
        /// <returns>Сведения о пользователе.</returns>
        UserItem GetUserInfo(User user, bool generateAccessToken = false);
    }
}
