using System;
using System.Collections.Generic;

namespace ECommerce.Shared.Models;

public partial class Store
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
}
