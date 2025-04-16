using AutoMapper;
using CloudinaryDotNet.Actions;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
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
            var storeProductRepo = _unitOfWork.GetRepository<StoreProduct>();
            var productRepo = _unitOfWork.GetRepository<Product>();
            var storeRepo = _unitOfWork.GetRepository<Store>();

            // Check nếu StoreProduct đã tồn tại
            var existingStoreProduct = await storeProductRepo.SingleOrDefaultAsync(
                predicate: x => x.StoreId == storeProductRequest.StoreId && x.ProductId == storeProductRequest.ProductId
            );
            if (existingStoreProduct != null)
            {
                throw new EntityAlreadyExistsException($"StoreProduct with StoreId {storeProductRequest.StoreId} and ProductId {storeProductRequest.ProductId} already exists.");
            }

            // Check Product tồn tại và đang active
            var product = await productRepo.SingleOrDefaultAsync(predicate: x => x.Id == storeProductRequest.ProductId);
            if (product == null)
            {
                throw new EntityNotFoundException($"Product with ID {storeProductRequest.ProductId} not found.");
            }

            if (product.Status != ProductEnum.ProductStatus.Active.ToString())
            {
                throw new InvalidOperationException($"Product with ID {storeProductRequest.ProductId} is not active.");
            }

            // Check tồn kho tổng có đủ để phân bổ không
            if (product.Stock < storeProductRequest.Stock)
            {
                throw new InvalidOperationException($"Not enough stock in warehouse to allocate. Available: {product.Stock}, Requested: {storeProductRequest.Stock}");
            }

            // Check Store tồn tại
            var store = await storeRepo.SingleOrDefaultAsync(predicate: x => x.Id == storeProductRequest.StoreId);
            if (store == null)
            {
                throw new EntityNotFoundException($"Store with ID {storeProductRequest.StoreId} not found.");
            }

            // Trừ kho tổng
            product.Stock -= storeProductRequest.Stock;
            productRepo.UpdateAsync(product);

            // Tạo StoreProduct mới
            var newStoreProduct = _mapper.Map<StoreProduct>(storeProductRequest);
            newStoreProduct.Id = Guid.NewGuid();

            await storeProductRepo.InsertAsync(newStoreProduct);

            // Commit transaction
            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to create store product and update stock.");
            }

            // Trả kết quả
            var response = _mapper.Map<StoreProductResponse>(newStoreProduct);
            return response;
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
            return product;
          
        }

        public async Task<StoreProductDetailResponse> GetStoreProductById(Guid storeId, Guid productId)
        {
            var storeProduct = await _unitOfWork.GetRepository<StoreProduct>().SingleOrDefaultAsync(
                predicate: x => x.StoreId == storeId && x.ProductId == productId,
                include: x => x.Include(sp => sp.Product).Include(sp => sp.Store)
            );

            if (storeProduct == null)
            {
                throw new EntityNotFoundException($"StoreProduct with StoreId {storeId} and ProductId {productId} not found.");
            }

            var storeProductResponse = _mapper.Map<StoreProductDetailResponse>(storeProduct);
            storeProductResponse.StoreName = storeProduct.Store.Name;
            storeProductResponse.ProductName = storeProduct.Product.Name;
            return storeProductResponse;
        }

        public async Task<StoreProductResponse> UpdateStoreProduct(Guid id, StoreProductRequest storeProductRequest)
        {
            var storeProductRepo = _unitOfWork.GetRepository<StoreProduct>();
            var productRepo = _unitOfWork.GetRepository<Product>();
            var storeRepo = _unitOfWork.GetRepository<Store>();

            // 1. Lấy StoreProduct hiện tại
            var storeProduct = await storeProductRepo.SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (storeProduct == null)
                throw new EntityNotFoundException($"StoreProduct with ID {id} not found.");

            // 2. Check Product tồn tại
            var product = await productRepo.SingleOrDefaultAsync(predicate: x => x.Id == storeProductRequest.ProductId);
            if (product == null)
                throw new EntityNotFoundException($"Product with ID {storeProductRequest.ProductId} not found.");

            // 3. Check Store tồn tại
            var store = await storeRepo.SingleOrDefaultAsync(predicate: x => x.Id == storeProductRequest.StoreId);
            if (store == null)
                throw new EntityNotFoundException($"Store with ID {storeProductRequest.StoreId} not found.");

            // 4. Check trùng combination (StoreId + ProductId)
            var duplicateStoreProduct = await storeProductRepo.SingleOrDefaultAsync(
                predicate:
                x => x.StoreId == storeProductRequest.StoreId &&
                     x.ProductId == storeProductRequest.ProductId &&
                     x.Id != id
            );
            if (duplicateStoreProduct != null)
                throw new DataConflictException($"Another StoreProduct with the same StoreId and ProductId already exists.");

            // 5. Nếu có thay đổi stock: kiểm tra chênh lệch và đảm bảo kho tổng còn đủ
            int stockDifference = storeProductRequest.Stock - storeProduct.Stock;
            if (stockDifference > 0)
            {
                if (product.Stock < stockDifference)
                {
                    throw new InvalidOperationException($"Not enough stock in central warehouse. Needed: {stockDifference}, Available: {product.Stock}");
                }

                // Trừ kho tổng
                product.Stock -= stockDifference;
                 productRepo.UpdateAsync(product);
            }
            else if (stockDifference < 0)
            {
                // Có thể cộng trả về kho tổng nếu cần
                product.Stock += Math.Abs(stockDifference);
                productRepo.UpdateAsync(product);
            }

            // 6. Cập nhật các giá trị
            storeProduct.StoreId = storeProductRequest.StoreId;
            storeProduct.ProductId = storeProductRequest.ProductId;
            storeProduct.Stock = storeProductRequest.Stock;
            storeProduct.Price = storeProductRequest.Price;

            storeProductRepo.UpdateAsync(storeProduct);

            if (await _unitOfWork.CommitAsync() <= 0)
                throw new DataConflictException("Failed to update store product.");

            var response = _mapper.Map<StoreProductResponse>(storeProduct);
            return response;
        }

    }
}
