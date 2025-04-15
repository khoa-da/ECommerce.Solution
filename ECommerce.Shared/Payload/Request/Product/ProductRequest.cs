using ECommerce.Shared.Enums;
using ECommerce.Shared.Payload.Request.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Payload.Request.Product
{
    public class ProductRequest
    {
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public GenderEnum? Gender { get; set; }
        public SizeEnum? Size { get; set; }
        public int Stock { get; set; }
        public BrandEnum? Brand { get; set; }
        public string? Sku { get; set; }
        public string? Tags { get; set; }
        public string? Material { get; set; }
        public List<string>? ProductImageBase64 { get; set; }
    }
}
