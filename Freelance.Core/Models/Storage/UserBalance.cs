using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelance.Core.Models.Storage;

/// <summary>
/// Счёт пользователя.
/// </summary>
public class UserBalance
{
    /// <summary>
    /// ИД пользователя.
    /// </summary>
    [Key]
    public int UserId { get; set; }

    /// <summary>
    /// Текущий баланс.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Balance { get; set; }

    /// <summary>
    /// Дата и время создания записи.
    /// </summary>
    [Required]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Дата и время обновления записи.
    /// </summary>
    [Required]
    public DateTimeOffset Updated { get; set; }
}
