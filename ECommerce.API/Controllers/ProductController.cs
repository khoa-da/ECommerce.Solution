using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Response.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class ProductController : BaseController<ProductController>
    {
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService) : base(logger)
        {
            _productService = productService;
        }

        [HttpPost(ApiEndPointConstant.Product.ProductsEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductResponse))]
        public async Task<IActionResult> Create([FromForm] ProductRequest request)
        {
            var response = await _productService.Create(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _productService.GetById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductsEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductResponse>))]
        public async Task<IActionResult> GetAll(string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _productService.GetAll(search, orderBy, page, size);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductByBrandIdEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductResponse>))]
        public async Task<IActionResult> GetByBrandId(BrandEnum brand, int page = 1, int size = 10)
        {
            var response = await _productService.GetByBrand(brand.ToString(), page, size);
            return Ok(response);
        }

    }
}
