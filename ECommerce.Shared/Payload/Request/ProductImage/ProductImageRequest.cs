namespace ECommerce.Shared.Payload.Request.ProductImage
{
    public class ProductImageRequest
    {
        public Guid ProductId { get; set; }

        public string Base64Image { get; set; } = null!;




    }
}
