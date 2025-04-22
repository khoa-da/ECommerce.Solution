namespace ECommerce.Shared.Payload.Response.OrderItem
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public string? Gender { get; set; }

        public string? Size { get; set; }

        public string? Brand { get; set; }

        public string? Sku { get; set; }

        public string? Tags { get; set; }

        public string? Material { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
