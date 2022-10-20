using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Orders
{
    /// <summary>
    /// Ответ на запрос списка заказов услуг.
    /// </summary>
    public class OrderListResponse
    {
        /// <summary>
        /// Общее количество элементов.
        /// </summary>
        [Required]
        public int TotalCount { get; set; }

        /// <summary>
        /// Элементы ответа.
        /// </summary>
        [Required]
        public OrderItem[] Items { get; set; } = Array.Empty<OrderItem>();
    }
}
