using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Store;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Shared.Payload.Response.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class StoreService : BaseService<StoreService>, IStoreService
    {
        public StoreService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<StoreService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<StoreResponse> Create(StoreRequest request)
        {

            var store = _mapper.Map<Store>(request);
            store.Id = Guid.NewGuid();
            store.CreatedDate = DateTime.UtcNow.AddHours(7);
            store.Status = StoreEnum.Status.Active.ToString();
            store.UpdatedDate = DateTime.UtcNow.AddHours(7);

            await _unitOfWork.GetRepository<Store>().InsertAsync(store);

            if(request.ProductIds != null && request.ProductIds.Count > 0)
            {
                foreach (var productId in request.ProductIds)
                {
                    var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id == productId);
                    if (product == null)
                    {
                        throw new EntityNotFoundException($"Product with ID {productId} not found.");
                    }
                    var storeProduct = new StoreProduct
                    {
                        Id = Guid.NewGuid(),
                        StoreId = store.Id,
                        ProductId = productId,
                        Stock = 0,
                        Price = product.Price
                    };
                    await _unitOfWork.GetRepository<StoreProduct>().InsertAsync(storeProduct);
                }
            }

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to create store.");
            }
            var storeResponse = _mapper.Map<StoreResponse>(store);
            if (request.IncludeProductsInResponse && request.ProductIds != null && request.ProductIds.Any())
            {
                storeResponse.Products = await GetStoreProducts(store.Id);
            }

            return  storeResponse;
        }
        private async Task<List<ProductResponse>> GetStoreProducts(Guid storeId)
        {
            var storeProductRepository = _unitOfWork.GetRepository<StoreProduct>();
            var productRepository = _unitOfWork.GetRepository<Product>();

            var storeProducts = await storeProductRepository.GetListAsync(predicate: sp => sp.StoreId == storeId);
            var productIds = storeProducts.Select(sp => sp.ProductId).ToList();

            var products = await productRepository.GetListAsync(predicate: p => productIds.Contains(p.Id));

            return _mapper.Map<List<ProductResponse>>(products);
        }

        public async Task<bool> Delete(Guid id)
        {
            var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (store == null)
            {
                throw new EntityNotFoundException("Store not found.");
            }
            store.Status = StoreEnum.Status.Deleted.ToString();
            store.UpdatedDate = DateTime.UtcNow.AddHours(7);
            _unitOfWork.GetRepository<Store>().UpdateAsync(store);

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to delete store.");

            }
            return true;
        }

        public async Task<IPaginate<StoreResponse>> GetAllStore(string? search, string? orderBy, int page, int size)
        {
            search = search?.Trim().ToLower();
            Func<IQueryable<Store>, IOrderedQueryable<Store>> orderByFunc = stores => stores.OrderByDescending(s => s.CreatedDate);

            var storeRepo = _unitOfWork.GetRepository<Store>();

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name_asc":
                        orderByFunc = stores => stores.OrderBy(s => s.Name);
                        break;
                    case "name_desc":
                        orderByFunc = stores => stores.OrderByDescending(s => s.Name);
                        break;
                    case "createddate_asc":
                        orderByFunc = stores => stores.OrderBy(s => s.CreatedDate);
                        break;
                    case "createddate_desc":
                        orderByFunc = stores => stores.OrderByDescending(s => s.CreatedDate);
                        break;
                }
            }
            var result = await storeRepo.GetPagingListAsync(
                selector: x => _mapper.Map<StoreResponse>(x),
                predicate: string.IsNullOrEmpty(search) ? x => true : x => x.Name.ToLower().Contains(search) || x.Address.ToLower().Contains(search),
                orderBy: orderByFunc,
                page: page,
                size: size
            );
            return result;    
        }

        public async Task<StoreResponse> GetById(Guid id)
        {
            var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (store == null)
            {
                throw new EntityNotFoundException("Store not found.");
            }
            var storeResponse = _mapper.Map<StoreResponse>(store);
            return storeResponse;
            
        }

        public async Task<StoreResponse> Update(Guid id, StoreRequest request)
        {
            var storeRepository = _unitOfWork.GetRepository<Store>();
            var store = await storeRepository.SingleOrDefaultAsync(predicate: x => x.Id == id);

            if (store == null)
            {
                throw new EntityNotFoundException("Store not found.");
            }

            // Update store properties
            store = _mapper.Map(request, store);
            store.UpdatedDate = DateTime.UtcNow.AddHours(7);

            storeRepository.UpdateAsync(store);

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to update store.");
            }

            var storeResponse = _mapper.Map<StoreResponse>(store);
            return storeResponse;
        }
    }
}
