using System;
using System.Collections.Generic;

namespace ECommerce.Shared.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Gender { get; set; }

    public string? Size { get; set; }

    public int Stock { get; set; }

    public string? Brand { get; set; }

    public string? Sku { get; set; }

    public string? Tags { get; set; }

    public string? Material { get; set; }

    public string? Address { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
}
