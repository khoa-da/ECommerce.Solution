using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> Create(OrderRequest order);
        Task<OrderResponse> Update(Guid id, OrderRequest order);
        Task<bool> Delete(Guid id);
        Task<OrderResponse> GetById(Guid id);
        Task<IPaginate<OrderResponse>> GetAll(string? search, string? orderBy, int page, int size);
        Task<IPaginate<OrderResponse>> GetByUserId(Guid userId, string? search, string? orderBy, int page, int size);
        Task<OrderResponse> ChangeStatus(Guid id, string status);


    }
}
    