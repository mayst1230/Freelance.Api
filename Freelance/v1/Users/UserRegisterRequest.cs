using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Users;

/// <summary>
/// Запрос на регистрацию пользователя.
/// </summary>
public class UserRegisterRequest : UserEditRequest
{
    /// <summary>
    /// Роль.
    /// </summary>
    [Required]
    public UserRole Role { get; set; }
}
