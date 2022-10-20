using Freelance.Api.v1.ReferenceItems;
using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Orders;

/// <summary>
/// Заказ услуги.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Уникальный ИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// ИД заказчика.
    /// </summary>
    [Required]
    public UserReferenceItem Customer { get; set; } = null!;

    /// <summary>
    /// ИД исполнителя.
    /// </summary>
    public UserReferenceItem? Contractor { get; set; }

    /// <summary>
    /// Заголовок.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание.
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Статус.
    /// </summary>
    [Required]
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Файл заказчика.
    /// </summary>
    public FileReferenceItem? CustomerFile { get; set; }

    /// <summary>
    /// Файл исполнителя.
    /// </summary>
    public FileReferenceItem? ContractorFile { get; set; }

    /// <summary>
    /// Стоимость.
    /// </summary>
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
    public bool? IsDeleted { get; set; }

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
