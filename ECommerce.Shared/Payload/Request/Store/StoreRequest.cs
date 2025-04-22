namespace ECommerce.Shared.Payload.Request.Store
{
    public class StoreRequest
    {
        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
        public List<Guid>? ProductIds { get; set; }
        public bool IncludeProductsInResponse { get; set; } = false;
    }
}
