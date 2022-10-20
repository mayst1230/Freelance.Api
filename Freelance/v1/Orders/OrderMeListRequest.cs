using Freelance.Core.Models.Storage;

namespace Freelance.Api.v1.Orders
{
    /// <summary>
    /// Запрос на получение списка собственных заказов услуг.
    /// </summary>
    public class OrderMeListRequest : OrderListExecutionRequest
    {
        /// <summary>
        /// Статус заказа услуги.
        /// </summary>
        public OrderStatus? Status { get; set; }

        /// <summary>
        /// Запись удалена.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
