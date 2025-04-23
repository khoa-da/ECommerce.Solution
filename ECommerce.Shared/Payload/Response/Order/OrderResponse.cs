using ECommerce.Shared.Payload.Response.OrderItem;

namespace ECommerce.Shared.Payload.Response.Order
{
    public class OrderResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; }

        public string? StorePhoneNumber { get; set; }

        public string? OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string? ShippingAddress { get; set; }

        public string? BillingAddress { get; set; }

        public string? PaymentStatus { get; set; }

        public string? OrderStatus { get; set; }
        public string? PaymentMethod { get; set; }

        public string? ShippingMethod { get; set; }
        public List<OrderItemResponse>? OrderItems { get; set; }
        public string? Notes { get; set; }
    }
}
