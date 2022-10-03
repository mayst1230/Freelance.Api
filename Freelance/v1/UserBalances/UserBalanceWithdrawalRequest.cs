using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.UserBalances;

/// <summary>
/// Запрос на вывод средств со счета пользователя.
/// </summary>
public class UserBalanceWithdrawalRequest
{
    /// <summary>
    /// Номер карты.
    /// </summary>
    [Required]
    [MinLength(16)]
    [MaxLength(16)]
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Сумма списания.
    /// </summary>
    [Required]
    public decimal AmountWithdrawal { get; set; }
}
