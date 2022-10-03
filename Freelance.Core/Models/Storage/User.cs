using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelance.Core.Models.Storage;

/// <summary>
/// Пользователь.
/// </summary>
public class User
{
    /// <summary>
    /// ИД.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Уникальный ИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Пароль.
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Имя.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Отчество.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string MiddleName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email-адрес.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Роль.
    /// </summary>
    [Required]
    public UserRole Role { get; set; }

    /// <summary>
    /// ИД файла с фото профиля.
    /// </summary>
    public int? PhotoFileId { get; set; }

    /// <summary>
    /// Общий рейтинг.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(2, 1)")]
    public decimal Rating { get; set; }

    /// <summary>
    /// Код для восстановления пароля.
    /// </summary>
    public string? RestorePasswordCode { get; set; }

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
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Дата и время обновления записи.
    /// </summary>
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Файл с фото профиля.
    /// </summary>
    public virtual File? PhotoFile { get; set; }
}

public enum UserRole
{
    /// <summary>
    /// Администратор.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Исполнитель.
    /// </summary>
    Contractor = 2,

    /// <summary>
    /// Заказчик.
    /// </summary>
    Customer = 3,
}
