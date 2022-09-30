using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Users;

/// <summary>
/// Пользователь.
/// </summary>
public class UserItem
{
    /// <summary>
    /// Уникальный ИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Пароль.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Роль.
    /// </summary>
    [Required]
    public UserRole Role { get; set; }

    /// <summary>
    /// Email-адрес.
    /// </summary>
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Рейтинг.
    /// </summary>
    public decimal? Rating { get; set; }

    /// <summary>
    /// ИД файла с фото профиля.
    /// </summary>
    public int? PhotoFileId { get; set; }

    /// <summary>
    /// Пользователь заблокирован.
    /// </summary>
    [Required]
    public bool IsBanned { get; set; }

    /// <summary>
    /// Пользователь удален.
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Дата и время создания записи.
    /// </summary>
    [Required]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Дата и время обновления записи.
    /// </summary>
    [Required]
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Токен доступа.
    /// </summary>
    public string? AccessToken { get; set; }
}
