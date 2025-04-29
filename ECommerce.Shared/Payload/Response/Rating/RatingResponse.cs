using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.Rating
{
    public class RatingResponse
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public string? Email { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
