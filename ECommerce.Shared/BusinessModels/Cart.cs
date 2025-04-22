using System.Text.Json.Serialization;

namespace ECommerce.Shared.BusinessModels
{
    public class Cart
    {
        public string Id { get; set; }
        public Guid UserId { get; set; } // Guid.Empty đối với guest cart
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public decimal TotalAmount => CalculateTotal();

        [JsonIgnore]
        public bool IsGuestCart => UserId == Guid.Empty;

        [JsonIgnore]
        public int ItemCount => Items.Count;

        [JsonIgnore]
        public int TotalQuantity
        {
            get
            {
                int total = 0;
                foreach (var item in Items)
                {
                    total += item.Quantity;
                }
                return total;
            }
        }

        private decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.TotalPrice;
            }
            return total;
        }
    }
}
