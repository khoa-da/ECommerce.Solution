using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Response.User
{
    public class UserResponse
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Role { get; set; } = null!;

        public DateTime RegisteredDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public string Status { get; set; }
    }
}
