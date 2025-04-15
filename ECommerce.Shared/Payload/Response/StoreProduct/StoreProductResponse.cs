using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.StoreProduct
{
    public class StoreProductResponse
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public Guid ProductId { get; set; }

        public int Stock { get; set; }

        public decimal? Price { get; set; }
    }
}
