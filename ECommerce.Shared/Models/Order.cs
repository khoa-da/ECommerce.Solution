﻿namespace ECommerce.Shared.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? StoreId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? ShippingAddress { get; set; }

    public string? BillingAddress { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string? ShippingMethod { get; set; }

    public string? Notes { get; set; }

    public string? Reason { get; set; }

    public Guid? HumanCancel { get; set; }

    public DateTime? CancelAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Store? Store { get; set; }

    public virtual User User { get; set; } = null!;
}
