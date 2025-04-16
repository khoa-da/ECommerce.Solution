using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.StoreProduct;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Shared.Payload.Response.StoreProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IStoreProductService
    {
        Task<StoreProductResponse> CreateStoreProduct(StoreProductRequest storeProductRequest);
        Task<bool> DeleteStoreProduct(Guid storeId, Guid productId);
        Task<StoreProductResponse> UpdateStoreProduct(Guid id, StoreProductRequest storeProductRequest);
        Task<StoreProductDetailResponse> GetStoreProductById(Guid storeId, Guid productId);
        Task<IPaginate<ProductResponse>> GetAllProductsByStoreId(Guid storeId, int page, int size);
    }
}
