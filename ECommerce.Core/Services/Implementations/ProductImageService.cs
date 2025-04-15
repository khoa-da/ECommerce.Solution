using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.ProductImage;
using ECommerce.Shared.Payload.Response.ProductImage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class ProductImageService : BaseService<ProductImageService>, IProductImageService
    {
        private readonly IImageService _cloudinaryService;
        public ProductImageService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<ProductImageService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageService cloudinaryService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ProductImageResponse> Create(ProductImageRequest request)
        {
            var product = await _unitOfWork.GetRepository<Product>().AnyAsync(predicate: p => p.Id == request.ProductId);
            if (!product)
            {
                throw new EntityNotFoundException("Product not found.");
            }
            var productImage = _mapper.Map<ProductImage>(request);
            productImage.Id = Guid.NewGuid();

            if (request.Base64Image != null)
            {
                UploadImgRequest uploadImgRequest = new UploadImgRequest
                {
                    Base64Image = request.Base64Image,
                    FileName = request.ProductId + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".jpg"
                };
                productImage.ImageUrl = await _cloudinaryService.UploadImage(uploadImgRequest);
            }

            var mainImage = await _unitOfWork.GetRepository<ProductImage>().AnyAsync(predicate: p => p.ProductId == request.ProductId && p.IsMain);
            if (mainImage)
            {
                productImage.IsMain = false;
            }
            else
            {
                productImage.IsMain = true;
            }

            var displayOrder = await _unitOfWork.GetRepository<ProductImage>().GetListAsync(predicate: p => p.ProductId == request.ProductId);
            if (displayOrder.Count() > 0)
            {
                productImage.DisplayOrder = displayOrder.Max(p => p.DisplayOrder) + 1;
            }
            else
            {
                productImage.DisplayOrder = 1;
            }
            productImage.Status = ProductImageEnum.Status.Active.ToString();

            await _unitOfWork.GetRepository<ProductImage>().InsertAsync(productImage);
            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new Exception("Failed to create product image.");
            }
            var productImageResponse = _mapper.Map<ProductImageResponse>(productImage);
            return productImageResponse;
        }

        public async Task<bool> Delete(Guid id)
        {
            var productImageRepo = _unitOfWork.GetRepository<ProductImage>();
            var image = await productImageRepo.SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (image == null)
            {
                throw new EntityNotFoundException("Product image not found.");
            }

            productImageRepo.DeleteAsync(image);
            return await _unitOfWork.CommitAsync() > 0;
        }


        public async Task<IPaginate<ProductImageResponse>> GetAll(string? search, string? orderBy, int page, int size)
        {
            search = search?.Trim().ToLower();

            Func<IQueryable<ProductImage>, IOrderedQueryable<ProductImage>> orderByFunc = q => q.OrderByDescending(x => x.DisplayOrder);
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "displayorder_asc":
                        orderByFunc = q => q.OrderBy(x => x.DisplayOrder);
                        break;
                    case "displayorder_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.DisplayOrder);
                        break;
                    case "status_asc":
                        orderByFunc = q => q.OrderBy(x => x.Status);
                        break;
                    case "status_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Status);
                        break;
                }
            }

            var result = await _unitOfWork.GetRepository<ProductImage>().GetPagingListAsync(
                selector: x => _mapper.Map<ProductImageResponse>(x),
                predicate: string.IsNullOrEmpty(search) ? x => true : x => x.ImageUrl.ToLower().Contains(search),
                orderBy: orderByFunc,
                page: page,
                size: size
            );

            return result;
        }


        public async Task<ProductImageResponse> GetById(Guid id)
        {
            var image = await _unitOfWork.GetRepository<ProductImage>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (image == null)
            {
                throw new EntityNotFoundException("Product image not found.");
            }

            return _mapper.Map<ProductImageResponse>(image);
        }


        public async Task<List<ProductImageResponse>> GetByProductId(Guid productId)
        {
            var images = await _unitOfWork.GetRepository<ProductImage>().GetListAsync(predicate: x => x.ProductId == productId);
            return images.Select(x => _mapper.Map<ProductImageResponse>(x)).ToList();
        }


        public async Task<ProductImageResponse> SetDisplayOrder(Guid id, int displayOrder)
        {
            var repo = _unitOfWork.GetRepository<ProductImage>();
            var image = await repo.SingleOrDefaultAsync( predicate: x => x.Id == id);
            if (image == null)
            {
                throw new EntityNotFoundException("Product image not found.");
            }

            image.DisplayOrder = displayOrder;
            repo.UpdateAsync(image);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ProductImageResponse>(image);
        }


        public async Task<ProductImageResponse> SetMainImage(Guid id)
        {
            var repo = _unitOfWork.GetRepository<ProductImage>();
            var image = await repo.SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (image == null)
            {
                throw new EntityNotFoundException("Product image not found.");
            }

            // Unset all other main images for this product
            var allImages = await repo.GetListAsync(predicate: x => x.ProductId == image.ProductId);
            foreach (var img in allImages)
            {
                img.IsMain = img.Id == id;
                repo.UpdateAsync(img);
            }

            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProductImageResponse>(image);
        }


        public Task<ProductImageResponse> Update(Guid id, ProductImageRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
