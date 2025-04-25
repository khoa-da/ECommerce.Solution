using ECommerce.Shared.Payload.Response.OrderItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.Order
{
    public class CancelOrderResponse
    {
        public Guid Id { get; set; }

        public string? OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Reason { get; set; }

        public Guid? HumanCancel { get; set; }

        public DateTime? CancelAt { get; set; }

    }
}
