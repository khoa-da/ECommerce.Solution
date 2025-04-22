namespace ECommerce.Shared.Payload.Request.Category
{
    public class CategoryRequest
    {
        public string Name { get; set; } = null!;

        public Guid? ParentId { get; set; }

        public string? Description { get; set; }
    }
}
