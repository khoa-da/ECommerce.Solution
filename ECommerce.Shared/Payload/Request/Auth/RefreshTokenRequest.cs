using System.ComponentModel.DataAnnotations;

namespace ECommerce.Shared.Payload.Request.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "RefreshToken is required")]
        public string? RefreshToken { get; set; }
    }
}
