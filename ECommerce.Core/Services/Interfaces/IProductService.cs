using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Response.Product;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IProductService
    {
        Task<CreateProductResponse> Create(ProductRequest request);
        Task<ProductDetailResponse> GetById(Guid id);
        Task<ProductResponse> Update(Guid id, UpdateProductRequest request);
        Task<bool> Delete(Guid id);
        Task<IPaginate<ProductResponse>> GetByCategoryId(Guid categoryId, string? search, string? orderBy, int page, int size);
        Task<IPaginate<ProductResponse>> GetByBrand(string brand, int page, int size);
        Task<IPaginate<ProductResponse>> GetAll(string? search, string? orderBy, string? status, int page, int size);
        Task<Cart> AddToCart(Guid productId, int quantity, Guid storeId);
        Task<ProductDetailResponse> GetProductByProductIdAndStoreId(Guid productId, Guid storeId);
        Task<IPaginate<ProductResponse>> GetProductByParentsCategory(Guid parentCategoryId, string? search, string? orderBy, int page, int size);
    }
}
