namespace Freelance.Api.Exceptions;

/// <summary>
/// Класс, описывающий то, что данные не найдены.
/// </summary>
public class ApiNotFoundException : ApiException
{
    /// <summary>
    /// Исключение API, что данные не найдены.
    /// </summary>
    /// <param name="message">Описание проблемы.</param>
    public ApiNotFoundException(string message = "") : base(message)
    {
    }

    /// <summary>
    /// Исключение API, что данные не найдены.
    /// </summary>
    /// <param name="message">Описание проблемы.</param>
    /// <param name="modelField">Поле модели.</param>
    public ApiNotFoundException(string message, string modelField) : base(message, modelField)
    {
    }
}
