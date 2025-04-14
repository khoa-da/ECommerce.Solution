using System;
using System.Collections.Generic;

namespace ECommerce.Shared.Models;

public partial class StoreProduct
{
    public Guid Id { get; set; }

    public Guid StoreId { get; set; }

    public Guid ProductId { get; set; }

    public int Stock { get; set; }

    public decimal? Price { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
