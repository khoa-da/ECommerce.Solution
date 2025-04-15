using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
