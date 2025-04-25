using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.BusinessModels
{
    public class CancelOrderFormViewModel
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
        public string Comments { get; set; }
    }
}
