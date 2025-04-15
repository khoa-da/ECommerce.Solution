using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Response.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IProductService
    {
        Task<CreateProductResponse> Create(ProductRequest request);
        Task<ProductResponse> GetById(Guid id);
        Task<ProductResponse> Update(Guid id, ProductRequest request);
        Task<bool> Delete(Guid id);
        Task<IPaginate<ProductResponse>> GetByCategoryId(Guid categoryId, int page, int size);
        Task<IPaginate<ProductResponse>> GetByBrand(string brand, int page, int size);
        Task<IPaginate<ProductResponse>> GetAll(string? search, string? orderBy, int page, int size);

    }
}
