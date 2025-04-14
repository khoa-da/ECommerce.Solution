using System;
using System.Collections.Generic;

namespace ECommerce.Shared.Models;

public partial class Rating
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }

    public int Score { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
