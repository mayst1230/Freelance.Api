using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Feedbacks;

/// <summary>
/// Ответ на запрос списка отзывов пользователя.
/// </summary>
public class FeedbackListResponse
{
    /// <summary>
    /// Общее количество элементов.
    /// </summary>
    [Required]
    public int TotalCount { get; set; }

    /// <summary>
    /// Элементы ответа.
    /// </summary>
    [Required]
    public FeedbackItem[] Items { get; set; } = Array.Empty<FeedbackItem>();
}
