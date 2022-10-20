using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelance.Core.Models.Storage;

/// <summary>
/// Операции со счётом пользователя.
/// </summary>
public class UserBalanceLog
{
    public UserBalanceLog()
    {
        UniqueIdentifier = Guid.NewGuid();
    }

    /// <summary>
    /// ИД.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Уникальный ИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// ИД пользователя-владельца счёта.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Баланс до проведения операции.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal BalanceBefore { get; set; }

    /// <summary>
    /// Баланс после проведения операции.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal BalanceAfter { get; set; }

    /// <summary>
    /// Дебет (приход).
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Debit { get; set; }

    /// <summary>
    /// Кредит (расход).
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
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

/// <summary>
/// Тип операции.
/// </summary>
public enum TypeGroup
{
    /// <summary>
    /// Пополнение баланса.
    /// </summary>
    Replenishment = 1,

    /// <summary>
    /// Вывод средств.
    /// </summary>
    Withdrawal = 2,

    /// <summary>
    /// Оплата заказа.
    /// </summary>
    Payment = 3,

    /// <summary>
    /// Получение средств с заказа.
    /// </summary>
    Receiving = 4,
}
