using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Orders;

/// <summary>
/// Запрос на создание заказа услуги.
/// </summary>
public class OrderCreateRequest
{
    /// <summary>
    /// Заголовок.
    /// </summary>
    [Required]
    [MinLength(10)]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание.
    /// </summary>
    [Required]
    [MinLength(20)]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ИД файла заказчика.
    /// </summary>
    public int? CustomerFileId { get; set; }

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
}
