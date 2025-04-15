using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Store;
using ECommerce.Shared.Payload.Response.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Mapper
{
    public class StoreMapper : Profile
    {
        public StoreMapper()
        {
            CreateMap<Store, StoreResponse>()
                .ReverseMap();
            CreateMap<StoreRequest, Store>()
                .ReverseMap();
        }
    }
}
