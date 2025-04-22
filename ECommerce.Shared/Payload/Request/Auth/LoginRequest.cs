using System.ComponentModel.DataAnnotations;

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
