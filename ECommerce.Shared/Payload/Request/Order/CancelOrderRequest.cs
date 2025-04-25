namespace ECommerce.Shared.Payload.Request.Order
{
    public class CancelOrderRequest
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }
}
