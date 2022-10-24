using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Users;

/// <summary>
/// Запрос на редактирование пользователя.
/// </summary>
public class UserEditRequest
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    public Guid UserUniqueIdentifier { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Пароль.
    /// </summary>
    [Required]
    [MinLength(6)]
    [MaxLength(250)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Имя.
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия.
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Отчество.
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string MiddleName { get; set; } = string.Empty;

    /// <summary>
    /// Email-адрес.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// ИД файла с фото профиля.
    /// </summary>
    public int? PhotoFileId { get; set; }
}
