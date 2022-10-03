using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Feedbacks;

/// <summary>
/// Запрос на создание отзыва.
/// </summary>
public class FeedbackCreateRequest
{
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
    public decimal UserRating { get; set; }
}
