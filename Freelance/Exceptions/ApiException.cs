namespace Freelance.Api.Exceptions;

/// <summary>
/// Класс, описывающий какое-либо бизнесовое исключение в API.
/// </summary>
public class ApiException : Exception
{
    /// <summary>
    /// Поле модели, к которому относится ошибка.
    /// </summary>
    public string? ModelField { get; private set; }

    /// <summary>
    /// Бизнесовое исключение API.
    /// </summary>
    /// <param name="message">Описание проблемы.</param>
    public ApiException(string? message = "") : base(message)
    {
    }

    /// <summary>
    /// Бизнесовое исключение API.
    /// </summary>
    /// <param name="message">Описание проблемы.</param>
    /// <param name="modelField">Поле модели.</param>
    public ApiException(string? message, string? modelField) : base(message)
    {
        ModelField = modelField;
    }
}

