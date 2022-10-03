using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.UserBalances;

/// <summary>
/// Запрос на пополнение счета пользователя.
/// </summary>
public class UserBalanceReplenishmentRequest
{
    /// <summary>
    /// Номер карты.
    /// </summary>
    [Required]
    [MinLength(16)]
    [MaxLength(16)]
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// CVC-код.
    /// </summary>
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public string Cvc { get; set; } = string.Empty;

    /// <summary>
    /// Сумма пополнения.
    /// </summary>
    [Required]
    public decimal AmountReplenishment { get; set; }
}
