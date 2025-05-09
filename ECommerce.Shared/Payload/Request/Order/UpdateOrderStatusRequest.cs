using ECommerce.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECommerce.Shared.Enums.OrderEnum;

namespace ECommerce.Shared.Payload.Request.Order
{
    public class UpdateOrderStatusRequest
    {
        public Guid orderId { get; set; }
        public string orderStatus { get; set; }
    }
}
