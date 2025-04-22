namespace ECommerce.Shared.Payload.Request.StoreProduct
{
    public class StoreProductRequest
    {
        public Guid StoreId { get; set; }

        public Guid ProductId { get; set; }

        public int Stock { get; set; }

        public decimal? Price { get; set; }
    }
}
