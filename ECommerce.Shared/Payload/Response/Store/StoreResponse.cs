using ECommerce.Shared.Payload.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.Store
{
    public class StoreResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string? Status { get; set; }
        public List<ProductResponse> Products { get; set; }
    }
}
