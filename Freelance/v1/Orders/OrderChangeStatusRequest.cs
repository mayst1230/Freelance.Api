using Freelance.Core.Models.Storage;

namespace Freelance.Api.v1.Orders;

/// <summary>
/// Запрос на изменение статуса заказа услуги.
/// </summary>
public class OrderChangeStatusRequest
{
    /// <summary>
    /// УИД заказа услуги.
    /// </summary>
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// Статус заказа услуги.
    /// </summary>
    public OrderStatus Status { get; set; }
}
