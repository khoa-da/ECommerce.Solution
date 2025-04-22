namespace ECommerce.Shared.BusinessModels
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}
