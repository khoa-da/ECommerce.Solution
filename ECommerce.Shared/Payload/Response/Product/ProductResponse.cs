using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.Product
{
    public class ProductResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

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

        public string? Status { get; set; }
    }
}
