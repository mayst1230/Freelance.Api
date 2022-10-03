using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.UserBalances;

/// <summary>
/// Ответ на запрос списка операций со счетом пользователя.
/// </summary>
public class UserBalanceLogListResponse
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
    public UserBalanceLogItem[] Items { get; set; } = Array.Empty<UserBalanceLogItem>();
}
