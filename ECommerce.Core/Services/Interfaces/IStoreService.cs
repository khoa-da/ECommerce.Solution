using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Store;
using ECommerce.Shared.Payload.Response.Store;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IStoreService
    {
        Task<StoreResponse> Create(StoreRequest request);
        Task<StoreResponse> GetById(Guid id);
        Task<StoreResponse> Update(Guid id, StoreRequest request);
        Task<bool> Delete(Guid id);
        Task<IPaginate<StoreResponse>> GetAllStore(string? search, string? orderBy, int page, int size);

    }
}
