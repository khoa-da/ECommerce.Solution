using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Response.Product;

namespace ECommerce.Core.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, CreateProductResponse>()
                .ReverseMap();
            CreateMap<ProductRequest, Product>()
                .ReverseMap();
            CreateMap<Product, ProductResponse>()
                .ReverseMap();
            CreateMap<Product, ProductDetailResponse>().ReverseMap();
        }
    }
}
