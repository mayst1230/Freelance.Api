using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.UserBalances;

/// <summary>
/// Элемент операции со счетом пользователя.
/// </summary>
public class UserBalanceLogItem
{
    /// <summary>
    /// Уникальный ИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// Баланс до проведения операции.
    /// </summary>
    [Required]
    public decimal BalanceBefore { get; set; }

    /// <summary>
    /// Баланс после проведения операции.
    /// </summary>
    [Required]
    public decimal BalanceAfter { get; set; }

    /// <summary>
    /// Дебет (приход).
    /// </summary>
    [Required]
    public decimal Debit { get; set; }

    /// <summary>
    /// Кредит (расход).
    /// </summary>
    [Required]
    public decimal Credit { get; set; }

    /// <summary>
    /// Тип операции.
    /// </summary>
    [Required]
    public TypeGroup Type { get; set; }

    /// <summary>
    /// Дата и время создания записи.
    /// </summary>
    [Required]
    public DateTimeOffset Created { get; set; }
}
