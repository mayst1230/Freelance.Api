using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelance.Core.Models.Storage;

/// <summary>
/// Заказ услуги.
/// </summary>
public class Order
{
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
    /// ИД заказчика.
    /// </summary>
    [Required]
    public int CustomerId { get; set; }

    /// <summary>
    /// ИД исполнителя.
    /// </summary>
    public int? ContractorId { get; set; }

    /// <summary>
    /// Заголовок.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание.
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Статус.
    /// </summary>
    [Required]
    public OrderStatus Status { get; set; }

    /// <summary>
    /// ИД файла заказчика.
    /// </summary>
    public int? CustomerFileId { get; set; }

    /// <summary>
    /// ИД файла исполнителя.
    /// </summary>
    public int? ContractorFileId { get; set; }

    /// <summary>
    /// Стоимость.
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    [Required]
    public decimal Price { get; set; }

    /// <summary>
    /// Дата и время начала действия заказа.
    /// </summary>
    [Required]
    public DateTimeOffset Started { get; set; }

    /// <summary>
    /// Дата и время окончания действия заказа.
    /// </summary>
    [Required]
    public DateTimeOffset Canceled { get; set; }

    /// <summary>
    /// Запись удалена.
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Статус заказа.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// На исполнение.
    /// </summary>
    Execution = 1,

    /// <summary>
    /// В разработке.
    /// </summary>
    Development = 2,

    /// <summary>
    /// К доработке.
    /// </summary>
    Rework = 3,

    /// <summary>
    /// На проверке.
    /// </summary>
    Review = 4,

    /// <summary>
    /// Сделан.
    /// </summary>
    Made = 5,
}
