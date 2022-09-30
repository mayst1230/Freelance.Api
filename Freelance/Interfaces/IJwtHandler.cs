using Freelance.Core.Models.Storage;

namespace Freelance.Api.Interfaces;

/// <summary>
/// Токен доступа.
/// </summary>
public interface IJwtHandler
{
    /// <summary>
    /// Генерация токена доступа.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <returns>Токен доступа.</returns>
    string GenerateToken(User user);
}
