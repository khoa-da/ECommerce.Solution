namespace ECommerce.Shared.Payload.Response.StoreProduct
{
    public class StoreProductDetailResponse
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }
        public string? StoreName { get; set; }

        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }

        public int Stock { get; set; }

        public decimal? Price { get; set; }
    }
}
