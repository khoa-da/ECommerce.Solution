using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Request.Rating
{
    public class RatingRequest
    {
        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }
    }
}
