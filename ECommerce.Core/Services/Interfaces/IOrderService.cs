using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;

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

        Task<OrderResponse> CreateV2(OrderRequest order);
        Task<IPaginate<OrderResponse>> GetAllByUserId(Guid userId, string? search, string? orderBy, int page, int size);
        Task<CancelOrderResponse> CancelOrder(Guid id, string? reason);
        Task<bool> UpdateOrderStatus(Guid id, string status);
    }
}
