using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Request.Product
{
    public class AddToCartProductRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public Guid StoreId { get; set; }
    }
}
