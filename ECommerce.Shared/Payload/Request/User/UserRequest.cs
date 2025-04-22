namespace ECommerce.Shared.Payload.Request.User
{
    public class UserRequest
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
