using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.ProductImage;
using ECommerce.Shared.Payload.Response.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Mapper
{
    public class ProductImageMapper : Profile
    {
        public ProductImageMapper()
        {
            CreateMap<ProductImage, ProductImageResponse>()
                .ReverseMap();
            CreateMap<ProductImageRequest, ProductImage>()
                .ReverseMap();
        }
    }
}
