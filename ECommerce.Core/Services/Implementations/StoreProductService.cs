using AutoMapper;
using CloudinaryDotNet.Actions;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.StoreProduct;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Shared.Payload.Response.StoreProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class StoreProductService : BaseService<StoreProductService>, IStoreProductService
    {
        public StoreProductService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<StoreProductService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<StoreProductResponse> CreateStoreProduct(StoreProductRequest storeProductRequest)
        {
            var storeProduct = await _unitOfWork.GetRepository<StoreProduct>().SingleOrDefaultAsync(predicate: x => x.StoreId == storeProductRequest.StoreId && x.ProductId == storeProductRequest.ProductId);
            if (storeProduct != null)
            {
                throw new EntityNotFoundException($"StoreProduct with StoreId {storeProductRequest.StoreId} and ProductId {storeProductRequest.ProductId} already exists.");
            }
            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id == storeProductRequest.ProductId);
            if (product == null)
            {
                throw new EntityNotFoundException($"Product with ID {storeProductRequest.ProductId} not found.");
            }
            var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(predicate: x => x.Id == storeProductRequest.StoreId);
            if (store == null)
            {
                throw new EntityNotFoundException($"Store with ID {storeProductRequest.StoreId} not found.");
            }
            var newStoreProduct = _mapper.Map<StoreProduct>(storeProductRequest);
            newStoreProduct.Id = Guid.NewGuid();

            await _unitOfWork.GetRepository<StoreProduct>().InsertAsync(newStoreProduct);
            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to create store product.");
            }
            var storeProductResponse = _mapper.Map<StoreProductResponse>(newStoreProduct);
            return storeProductResponse;
        }

        public async Task<bool> DeleteStoreProduct(Guid storeId, Guid productId)
        {
            var storeProduct = await _unitOfWork.GetRepository<StoreProduct>().SingleOrDefaultAsync(predicate: x => x.StoreId == storeId && x.ProductId == productId);
            if (storeProduct == null)
            {
                throw new EntityNotFoundException($"StoreProduct with StoreId {storeId} and ProductId {productId} not found.");
            }
            _unitOfWork.GetRepository<StoreProduct>().DeleteAsync(storeProduct);
            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to delete store product.");
            }
            return true;
        }

        public async Task<IPaginate<ProductResponse>> GetAllProductsByStoreId(Guid storeId, int page, int size)
        {
            var product = await _unitOfWork.GetRepository<StoreProduct>().GetPagingListAsync(
                selector: x => new ProductResponse
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    CategoryId = x.Product.CategoryId,
                    CategoryName = x.Product.Category.Name,
                    Description = x.Product.Description,
                    Price = x.Product.Price,
                    CreatedDate = x.Product.CreatedDate,
                    UpdatedDate = x.Product.UpdatedDate,
                    Gender = x.Product.Gender,
                    Size = x.Product.Size,
                    Stock = x.Stock,
                    Brand = x.Product.Brand,
                    Sku = x.Product.Sku,
                    Tags = x.Product.Tags,
                    Material = x.Product.Material,
                    Status = x.Product.Status
                },
                predicate: x => x.StoreId == storeId,
                include: x => x.Include(x => x.Product).ThenInclude(x => x.Category),
                page: page,
                size: size
                );
            throw new NotImplementedException();
        }

        public Task<StoreProductResponse> GetStoreProductById(Guid storeId, Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<StoreProductResponse> UpdateStoreProduct(Guid id, StoreProductRequest storeProductRequest)
        {
            throw new NotImplementedException();
        }
    }
}
