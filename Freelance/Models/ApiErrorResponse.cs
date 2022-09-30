namespace Freelance.Api.Models;

/// <summary>
/// Бизнесовая ошибка от API.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Список ошибок, где ключ - имя параметра, а значение - текст ошибок этого параметра.
    /// Если параметр string.Empty, то ошибка общая.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
