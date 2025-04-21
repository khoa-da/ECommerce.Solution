using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Product;

namespace ECommerce.Web.Services.Interfaces
{
    public interface IProductApiService
    {
        Task<IPaginate<ProductResponse>> GetAll(string? search, string? orderBy, int page, int size);
    }
}
