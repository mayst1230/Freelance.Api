using Freelance.Core.Models.Storage;

namespace Freelance.Api.v1.Orders
{
    /// <summary>
    /// Запрос на список заказов услуг в статусе "На исполнение".
    /// </summary>
    public class OrderListExecutionRequest
    {
        /// <summary>
        /// Количество записей.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Отступ от начала списка.
        /// </summary>
        public int? Offset { get; set; }
    }
}
