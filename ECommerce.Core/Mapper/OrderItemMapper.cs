using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.OrderItem;
using ECommerce.Shared.Payload.Response.OrderItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Mapper
{
    public class OrderItemMapper : Profile
    {
        public OrderItemMapper() {
            CreateMap<OrderItem, OrderItemResponse>().ReverseMap();
            CreateMap<OrderItemRequest, OrderItem>().ReverseMap();
        }
    }
}
