using Freelance.Core.Models.Storage;

namespace Freelance.Api.v1.UserBalances;

/// <summary>
/// Запрос на список операций со счетом пользователя.
/// </summary>
public class UserBalanceLogListRequest
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
    /// Тип операции.
    /// </summary>
    public TypeGroup? Type { get; set; }
}
