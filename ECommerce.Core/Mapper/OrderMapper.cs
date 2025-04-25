using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;

namespace ECommerce.Core.Mapper
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<Order, CancelOrderResponse>().ReverseMap();
            CreateMap<OrderRequest, Order>().ReverseMap();
        }
    }
}
