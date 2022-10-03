using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.UserBalances;

/// <summary>
/// Ответ на запрос операции со счетом пользователя.
/// </summary>
public class UserBalanceOperationResponse
{
    /// <summary>
    /// УИД пользователя.
    /// </summary>
    [Required]
    public Guid UserUniqueIdentifier { get; set; }

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
    /// Тип операции.
    /// </summary>
    [Required]
    public TypeGroup OperationType { get; set; }
}
