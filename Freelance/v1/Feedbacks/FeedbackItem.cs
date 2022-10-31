using Freelance.Api.v1.ReferenceItems;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Feedbacks;

/// <summary>
/// Элемент отзыва.
/// </summary>
public class FeedbackItem
{
    /// <summary>
    /// Уникальный ИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// Пользователь, к которому относится отзыв.
    /// </summary>
    [Required]
    public UserReferenceItem UserFeedback { get; set; } = null!;

    /// <summary>
    /// Текст.
    /// </summary>
    [Required]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Оценка пользователя, к которому относится отзыв.
    /// </summary>
    [Required]
    public decimal UserRating { get; set; }

    /// <summary>
    /// Пользователь, которым создана запись.
    /// </summary>
    [Required]
    public UserReferenceItem CreatedBy { get; set; } = null!;

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
}
