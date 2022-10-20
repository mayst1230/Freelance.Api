namespace Freelance.Api.v1.Orders
{
    /// <summary>
    /// Запрос на изменение заказа услуги.
    /// </summary>
    public class OrderEditRequest : OrderCreateRequest 
    { 
        /// <summary>
        /// ИД файла исполнителя.
        /// </summary>
        public int? ContractorFileId { get; set; }
    }
}
