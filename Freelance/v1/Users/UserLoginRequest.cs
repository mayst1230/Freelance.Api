using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Users;

/// <summary>
/// Запрос на авторизацию пользователя.
/// </summary>
public class UserLoginRequest
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Пароль.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
