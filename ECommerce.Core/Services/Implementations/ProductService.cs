using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Request.ProductImage;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Shared.Payload.Response.ProductImage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core.Services.Implementations
{
    public class ProductService : BaseService<ProductService>, IProductService
    {
        private readonly IImageService _imageService;
        private readonly ICartService _cartService;
        private readonly string _guestCartCookieName = "guest_cart_id";
        private readonly TimeSpan _guestCartCookieExpiry = TimeSpan.FromDays(7);

        public ProductService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<ProductService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageService imageService, ICartService cartService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _imageService = imageService;
            _cartService = cartService;
        }

        public async Task<CreateProductResponse> Create(ProductRequest request)
        {
            // Kiểm tra category tồn tại
            var category = await ValidateCategoryAsync(request.CategoryId);

            // Tạo và lưu product
            var product = await CreateAndSaveProductAsync(request);

            // Xử lý và lưu product images nếu có
            List<ProductImage> savedImages = new List<ProductImage>();
            if (request.ProductImageBase64 != null && request.ProductImageBase64.Count > 0)
            {
                savedImages = await ProcessAndSaveProductImagesAsync(product, request.ProductImageBase64);
            }

            // Tạo response
            var productResponse = _mapper.Map<CreateProductResponse>(product);
            productResponse.CategoryName = category.Name;

            // Map danh sách ảnh vào response
            if (savedImages.Any())
            {
                productResponse.ProductImageResponses = _mapper.Map<List<ProductImageResponse>>(savedImages);
            }

            return productResponse;
        }

        private async Task<Category> ValidateCategoryAsync(Guid categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(predicate: x => x.Id == categoryId);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            return category;
        }

        private async Task<Product> CreateAndSaveProductAsync(ProductRequest request)
        {
            var product = _mapper.Map<Product>(request);
            product.CreatedDate = DateTime.UtcNow.AddHours(7);
            product.UpdatedDate = DateTime.UtcNow.AddHours(7);
            product.Status = ProductEnum.ProductStatus.Active.ToString();

            await _unitOfWork.GetRepository<Product>().InsertAsync(product);
            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new Exception("Failed to create product");
            }

            return product;
        }

        private async Task<List<ProductImage>> ProcessAndSaveProductImagesAsync(Product product, List<string> base64Images)
        {
            int imageOrder = 1;
            bool isFirstImage = true;
            List<ProductImage> savedImages = new List<ProductImage>();

            foreach (var base64Image in base64Images)
            {
                if (!string.IsNullOrEmpty(base64Image))
                {
                    var productImage = await CreateProductImageAsync(product.Id, base64Image, isFirstImage, imageOrder++);
                    if (productImage != null)
                    {
                        savedImages.Add(productImage);
                        isFirstImage = false;
                    }
                }
            }

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new Exception("Failed to save product images");
            }

            return savedImages;
        }

        private async Task<ProductImage?> CreateProductImageAsync(Guid productId, string base64Image, bool isMain, int displayOrder)
        {
            var imageRequest = new UploadImgRequest
            {
                Base64Image = base64Image,
                FileName = productId.ToString() + DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") + ".jpg"
            };

            var imageUrl = await _imageService.UploadImage(imageRequest);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            var productImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ImageUrl = imageUrl,
                IsMain = isMain,
                DisplayOrder = displayOrder,
                Status = ProductImageEnum.Status.Active.ToString(),
            };

            await _unitOfWork.GetRepository<ProductImage>().InsertAsync(productImage);
            return productImage;
        }

        public async Task<bool> Delete(Guid id)
        {
            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            product.Status = ProductEnum.ProductStatus.Deleted.ToString();
            product.UpdatedDate = DateTime.UtcNow.AddHours(7);
            _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new Exception("Failed to delete product");
            }
            return true;
        }

        public async Task<IPaginate<ProductResponse>> GetProductByParentsCategory(Guid parentCategoryId, string? search, string? orderBy, int page, int size)
        {
            search = search?.Trim().ToLower();
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name_asc":
                        orderByFunc = q => q.OrderBy(x => x.Name);
                        break;
                    case "name_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Name);
                        break;
                    case "price_asc":
                        orderByFunc = q => q.OrderBy(x => x.Price);
                        break;
                    case "price_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Price);
                        break;
                    case "created_date_asc":
                        orderByFunc = q => q.OrderBy(x => x.CreatedDate);
                        break;
                    case "created_date_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            var products = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
                selector: x => new ProductResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Description = x.Description,
                    Price = x.Price,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    Gender = x.Gender,
                    Size = x.Size,
                    Stock = x.Stock,
                    Brand = x.Brand,
                    Sku = x.Sku,
                    Tags = x.Tags,
                    Material = x.Material,
                    Status = x.Status,
                    MainImage = x.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl
                },
                predicate: x => x.Category.ParentId == parentCategoryId &&
                                (string.IsNullOrEmpty(search) ||
                                 x.Name.ToLower().Contains(search) ||
                                 x.Description.ToLower().Contains(search) ||
                                 x.Brand.ToLower().Contains(search) ||
                                 x.Sku.ToLower().Contains(search) ||
                                 x.Tags.ToLower().Contains(search)),
                orderBy: orderByFunc,
                include: x => x.Include(x => x.Category).Include(x => x.ProductImages),
                page: page,
                size: size
            );
            return products;
        }
        public async Task<IPaginate<ProductResponse>> GetAll(string? search, string? orderBy, int page, int size)
        {
            search = search?.Trim().ToLower();

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name_asc":
                        orderByFunc = q => q.OrderBy(x => x.Name);
                        break;
                    case "name_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Name);
                        break;
                    case "price_asc":
                        orderByFunc = q => q.OrderBy(x => x.Price);
                        break;
                    case "price_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Price);
                        break;
                    case "created_date_asc":
                        orderByFunc = q => q.OrderBy(x => x.CreatedDate);
                        break;
                    case "created_date_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            var products = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
                selector: x => new ProductResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Description = x.Description,
                    Price = x.Price,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    Gender = x.Gender,
                    Size = x.Size,
                    Stock = x.Stock,
                    Brand = x.Brand,
                    Sku = x.Sku,
                    Tags = x.Tags,
                    Material = x.Material,
                    Status = x.Status,
                    MainImage = x.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl
                },
                predicate: x => string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search),
                orderBy: orderByFunc,
                page: page,
                size: size
            );
            return products;
        }

        public async Task<IPaginate<ProductResponse>> GetByBrand(string brand, int page, int size)
        {
            if (string.IsNullOrEmpty(brand))
            {
                throw new ArgumentException("Brand cannot be null or empty", nameof(brand));
            }

            var product = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
                selector: x => new ProductResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Description = x.Description,
                    Price = x.Price,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    Gender = x.Gender,
                    Size = x.Size,
                    Brand = x.Brand,
                    Sku = x.Sku,
                    Tags = x.Tags,
                    Material = x.Material,
                    Status = x.Status,
                    MainImage = x.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl
                },
                predicate: x => x.Brand != null && x.Brand.ToLower().Equals(brand.ToLower()),
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                include: x => x.Include(x => x.Category),
                page: page,
                size: size
            );
            return product;
        }

        public async Task<IPaginate<ProductResponse>> GetByCategoryId(Guid categoryId, string? search, string? orderBy, int page, int size)
        {
            search = search?.Trim().ToLower();

            // Define the order by function based on the orderBy parameter
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc;

            switch (orderBy?.ToLower())
            {
                case "name":
                    orderByFunc = q => q.OrderBy(x => x.Name);
                    break;
                case "name_desc":
                    orderByFunc = q => q.OrderByDescending(x => x.Name);
                    break;
                case "price":
                    orderByFunc = q => q.OrderBy(x => x.Price);
                    break;
                case "price_desc":
                    orderByFunc = q => q.OrderByDescending(x => x.Price);
                    break;
                case "created":
                    orderByFunc = q => q.OrderBy(x => x.CreatedDate);
                    break;
                case "created_desc":
                default:
                    orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                    break;
            }

            var product = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
                selector: x => new ProductResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Description = x.Description,
                    Price = x.Price,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    Gender = x.Gender,
                    Size = x.Size,
                    Stock = x.Stock,
                    Brand = x.Brand,
                    Sku = x.Sku,
                    Tags = x.Tags,
                    Material = x.Material,
                    Status = x.Status,
                    MainImage = x.ProductImages.FirstOrDefault(img => img.IsMain).ImageUrl
                },
                predicate: x => x.CategoryId == categoryId &&
                                (string.IsNullOrEmpty(search) ||
                                 x.Name.ToLower().Contains(search) ||
                                 x.Description.ToLower().Contains(search) ||
                                 x.Brand.ToLower().Contains(search) ||
                                 x.Sku.ToLower().Contains(search) ||
                                 x.Tags.ToLower().Contains(search)),
                orderBy: orderByFunc,
                include: x => x.Include(x => x.Category).Include(x => x.ProductImages),
                page: page,
                size: size
            );

            return product;
        }

        public async Task<ProductDetailResponse> GetById(Guid id)
        {
            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id == id, include: x => x.Include(x => x.Category).Include(x => x.ProductImages));
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            var productResponse = _mapper.Map<ProductDetailResponse>(product);
            productResponse.CategoryName = product.Category.Name;
            productResponse.ImageUrls = product.ProductImages.Select(x => x.ImageUrl).ToList();
            return productResponse;
        }

        public Task<ProductResponse> Update(Guid id, ProductRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductDetailResponse> GetProductByProductIdAndStoreId(Guid productId, Guid storeId)
        {
            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id == productId, include: x => x.Include(x => x.Category).Include(x => x.ProductImages));
            if (product == null)
            {
                throw new EntityNotFoundException("Product not found");
            }
            var storeProduct = await _unitOfWork.GetRepository<StoreProduct>().SingleOrDefaultAsync(predicate: x => x.ProductId == productId && x.StoreId == storeId);
            if (storeProduct == null)
            {
                throw new EntityNotFoundException("StoreProduct not found");
            }
            var productResponse = _mapper.Map<ProductDetailResponse>(product);
            productResponse.CategoryName = product.Category.Name;
            productResponse.Stock = storeProduct.Stock;
            productResponse.Price = storeProduct.Price.Value;
            productResponse.ImageUrls = product.ProductImages.Select(x => x.ImageUrl).ToList();
            return productResponse;
        }

        public async Task<Cart> AddToCart(Guid productId, int quantity, Guid storeId)
        {
            // Validate the product exists
            var product = await _unitOfWork.GetRepository<Product>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id == productId,
                    include: x => x.Include(p => p.ProductImages)
                );

            if (product == null)
            {
                throw new EntityNotFoundException("Product not found");
            }

            // Check stock availability in the specified store
            var storeProduct = await _unitOfWork.GetRepository<StoreProduct>()
                .SingleOrDefaultAsync(
                    predicate: x => x.ProductId == productId && x.StoreId == storeId
                );

            if (storeProduct == null)
            {
                throw new EntityNotFoundException("Product not available in this store");
            }

            if (storeProduct.Stock < quantity)
            {
                throw new InvalidOperationException("Not enough stock");
            }

            // Get main image URL safely using null conditional operator
            var mainImage = product.ProductImages?.FirstOrDefault(x => x.IsMain)?.ImageUrl;

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = storeProduct.Price.Value,
                Quantity = quantity,
                ImageUrl = mainImage
            };

            Cart cart;

            // Check if user is authenticated
            var userId = GetUserIdFromJwt();
            if (userId != null)
            {
                // User is logged in - get or create their cart
                cart = await _cartService.GetUserCartAsync(Guid.Parse(userId));
                if (cart == null)
                {
                    // Create a new cart for the user if one doesn't exist
                    cart = await _cartService.CreateCartAsync(Guid.Parse(userId));
                }
            }
            else
            {
                // Handle guest cart with cookies
                string guestId = GetCookieValue(_guestCartCookieName);
                if (string.IsNullOrEmpty(guestId))
                {
                    guestId = GenerateNewGuid();
                    // Set cookie for guest cart
                    SetCookie(
                        _guestCartCookieName,
                        guestId,
                        _guestCartCookieExpiry
                    );
                }
                cart = await _cartService.GetGuestCartAsync(guestId);
                if (cart == null)
                {
                    cart = await _cartService.CreateGuestCartAsync(guestId);
                }
            }

            // Add the item to the cart
            var updatedCart = await _cartService.AddToCartAsync(cart.Id, cartItem);
            return updatedCart;
        }

    }
}
