namespace ECommerce.Shared.Payload.Request.Product
{
    public class AddToCartProductRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public Guid StoreId { get; set; }
    }
}
