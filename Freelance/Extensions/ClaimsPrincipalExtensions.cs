using System.Security.Claims;

namespace Freelance.Api.Extensions;

/// <summary>
/// Методы расширения для информации о пользователе.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public const string CLAIM_USER_UUID = "uuid";

    /// <summary>
    /// Возвращает УИД пользователя.
    /// </summary>
    /// <param name="user">Сведения о пользователе.</param>
    /// <returns>УИД пользователя или null.</returns>
    public static Guid? GetUserUuid(this ClaimsPrincipal user)
    {
        if (Guid.TryParse(user.FindFirstValue(CLAIM_USER_UUID), out var userUuid))
        {
            return userUuid;
        }

        return null;
    }

    /// <summary>
    /// Возвращает роль пользователя.
    /// </summary>
    /// <param name="user">Сведения о пользователе.</param>
    /// <returns>Роль пользователя.</returns>
    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role);
    }

    /// <summary>
    /// Возвращает почту пользователя.
    /// </summary>
    /// <param name="user">Сведения о пользователе.</param>
    /// <returns>Почта пользователя.</returns>
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email);
    }
}
