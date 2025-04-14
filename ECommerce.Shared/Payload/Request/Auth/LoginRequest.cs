using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Request.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "UsernameOrEmail is required")]
        public string? UsernameOrEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
