namespace ECommerce.Shared.Payload.Response.ProductImage
{
    public class ProductImageResponse
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public bool IsMain { get; set; }

        public int DisplayOrder { get; set; }
        public string Status { get; set; } = null!;
    }
}
