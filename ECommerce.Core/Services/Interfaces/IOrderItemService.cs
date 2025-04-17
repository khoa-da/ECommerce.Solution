using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.OrderItem;
using ECommerce.Shared.Payload.Response.OrderItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemResponse> Create(OrderItemRequest orderItem);
        Task<OrderItemResponse> Update(Guid id, OrderItemRequest orderItem);
        Task<bool> Delete(Guid id);
        Task<OrderItemResponse> GetById(Guid id);
        Task<IPaginate<OrderItemResponse>> GetAll(string? search, string? orderBy, int page, int size);
        Task<IPaginate<OrderItemResponse>> GetByOrderId(Guid orderId, string? search, string? orderBy, int page, int size);
    }
}
