using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.ProductImage;
using ECommerce.Shared.Payload.Response.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IProductImageService
    {
        Task<ProductImageResponse> Create(ProductImageRequest request);
        Task<ProductImageResponse> GetById(Guid id);
        Task<ProductImageResponse> Update(Guid id, ProductImageRequest request);
        Task<bool> Delete(Guid id);
        Task<List<ProductImageResponse>> GetByProductId(Guid productId);
        Task<ProductImageResponse> SetMainImage(Guid id);
        Task<ProductImageResponse> SetDisplayOrder(Guid id, int displayOrder);
        Task<IPaginate<ProductImageResponse>> GetAll(string? search, string? orderBy, int page, int size);

    }
}
