using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Feedbacks;

/// <summary>
/// Ответ на запрос создания отзыва.
/// </summary>
public class FeedbackCreateResponse
{
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
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Оценка пользователя, к которому относится отзыв.
    /// </summary>
    [Required]
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
}
