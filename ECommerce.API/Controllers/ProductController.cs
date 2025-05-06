using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Shared.Payload.Response.ProductImage;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class ProductController : BaseController<ProductController>
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;
        public ProductController(ILogger<ProductController> logger, IProductService productService, IProductImageService productImageService) : base(logger)
        {
            _productService = productService;
            _productImageService = productImageService;
        }

        [HttpPost(ApiEndPointConstant.Product.ProductsEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductResponse))]
        public async Task<IActionResult> Create([FromForm] ProductRequest request)
        {
            var response = await _productService.Create(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDetailResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _productService.GetById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductByStoreIdEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDetailResponse))]
        public async Task<IActionResult> GetByStoreId(Guid storeId, Guid id)
        {
            var response = await _productService.GetProductByProductIdAndStoreId(id, storeId);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Product.ProductsEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductResponse>))]
        public async Task<IActionResult> GetAll(string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _productService.GetAll(search, orderBy, ProductEnum.ProductStatus.Active.ToString() ,page, size);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductsAdminEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductResponse>))]
        public async Task<IActionResult> GetAllForAdmin(string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _productService.GetAll(search, orderBy, null, page, size);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductByBrandIdEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductResponse>))]
        public async Task<IActionResult> GetByBrandId(BrandEnum brand, int page = 1, int size = 10)
        {
            var response = await _productService.GetByBrand(brand.ToString(), page, size);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Product.ImagesInProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductImageResponse>))]
        public async Task<IActionResult> GetImagesInProduct(Guid id)
        {
            var response = await _productImageService.GetByProductId(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Product.AddToCartEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Cart))]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartProductRequest addToCartProductRequest)
        {
            var cart = await _productService.AddToCart(addToCartProductRequest.ProductId, addToCartProductRequest.Quantity, addToCartProductRequest.StoreId);
            return Ok(cart);
        }
        [HttpPatch(ApiEndPointConstant.Product.ProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDetailResponse))]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductRequest request)
        {
            var response = await _productService.Update(id, request);
            return Ok(response);
        }

        [HttpDelete(ApiEndPointConstant.Product.ProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _productService.Delete(id);
            return Ok(response);
        }

    }
}
