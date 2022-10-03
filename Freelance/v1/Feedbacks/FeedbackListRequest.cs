namespace Freelance.Api.v1.Feedbacks;

/// <summary>
/// Запрос на список отзывов пользователя.
/// </summary>
public class FeedbackListRequest
{
    /// <summary>
    /// Количество записей.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Отступ от начала списка.
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// УИД пользователя, отзывы которого хотим получить.
    /// Если пустой, получаем отзывы текущего пользователя.
    /// </summary>
    public Guid? UserUniqueIdentifier { get; set; }
}
