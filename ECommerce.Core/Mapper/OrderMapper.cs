using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Mapper
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<OrderRequest, Order>().ReverseMap();
        }
    }
}
