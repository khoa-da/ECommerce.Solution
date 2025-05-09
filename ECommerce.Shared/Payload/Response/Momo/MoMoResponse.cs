using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.Momo
{
    public class MoMoResponse
    {
        public string partnerCode { get; set; }
        public Guid orderId { get; set; }
        public Guid requestId { get; set; }
        public long amount { get; set; }
        public string orderInfo { get; set; }
        public string orderType { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string payType { get; set; }
        public long responseTime { get; set; }
        public string extraData { get; set; }
        public string signature { get; set; }
    }
}
