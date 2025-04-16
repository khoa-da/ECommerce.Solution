using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.StoreProduct;
using ECommerce.Shared.Payload.Response.StoreProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Mapper
{
    public class StoreProductMapper : Profile
    {
        public StoreProductMapper()
        {
            CreateMap<StoreProduct, StoreProductResponse>().ReverseMap();
            CreateMap<StoreProduct, StoreProductDetailResponse>().ReverseMap();
            CreateMap<StoreProductRequest, StoreProduct>().ReverseMap();
        }
    }
}
