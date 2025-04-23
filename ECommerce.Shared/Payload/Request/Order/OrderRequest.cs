using ECommerce.Shared.BusinessModels;

namespace ECommerce.Shared.Payload.Request.Order
{
    public class OrderRequest
    {
        public Guid UserId { get; set; }

        //public Guid? StoreId { get; set; }

        public string? ShippingAddress { get; set; }

        public string? BillingAddress { get; set; }
        public string? PaymentMethod { get; set; }

        public string? ShippingMethod { get; set; }
        public List<CartItem> CartItems { get; set; }
        public string? Notes { get; set; }
    }
}
