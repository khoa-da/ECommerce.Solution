using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Category;
using ECommerce.Shared.Payload.Response.Category;
using ECommerce.Shared.Payload.Response.Product;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class CategoryController : BaseController<CategoryController>
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService, IProductService productService) : base(logger)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        [HttpPost(ApiEndPointConstant.Category.CategoriesEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> Create([FromBody] CategoryRequest request)
        {
            var response = await _categoryService.Create(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
        [HttpGet(ApiEndPointConstant.Category.CategoryEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _categoryService.GetById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Category.CategoriesEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<CategoryResponse>))]
        public async Task<IActionResult> GetAll(string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _categoryService.GetAll(search, orderBy, page, size);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Category.CategoryByParentIdEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<CategoryResponse>))]
        public async Task<IActionResult> GetByParentId(Guid? parentId, string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _categoryService.GetByParentId(parentId, search, orderBy, page, size);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Category.ProductsInCategoryEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductResponse>))]
        public async Task<IActionResult> GetProductsInCategory(Guid id, string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _productService.GetByCategoryId(id, search, orderBy, page, size);
            return Ok(response);
        }

    }
}
