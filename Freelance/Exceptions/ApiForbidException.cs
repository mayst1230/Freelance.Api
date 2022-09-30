namespace Freelance.Api.Exceptions;

/// <summary>
/// Класс, описывающий то, что нет доступа к данным.
/// </summary>
public class ApiForbidException : ApiException
{
    /// <summary>
    /// Исключение API, что нет доступа к данным.
    /// </summary>
    /// <param name="message">Описание проблемы.</param>
    public ApiForbidException(string? message = "") : base(message)
    {
    }
}
