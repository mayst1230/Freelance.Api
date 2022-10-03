using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelance.Core.Models.Storage;

/// <summary>
/// Отзыв.
/// </summary>
public class Feedback
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
    /// ИД пользователя, к которому относится отзыв.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Текст.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Оценка пользователя, к которому относится отзыв.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(2, 1)")]
    public decimal UserRating { get; set; }

    /// <summary>
    /// Запись удалена.
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Кем создана запись.
    /// </summary>
    [Required]
    public int CreatedBy { get; set; }

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
    /// Пользователь, создавший отзыв.
    /// </summary>
    public virtual User CreatedByUser { get; set; } = null!;

    /// <summary>
    /// Пользователь, к которому относится отзыв.
    /// </summary>
    public virtual User User { get; set; } = null!;
}

