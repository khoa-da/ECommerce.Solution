﻿namespace ECommerce.Shared.Payload.Request.OrderItem
{
    public class OrderItemRequest
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
