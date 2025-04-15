using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.ProductImage;
using ECommerce.Shared.Payload.Response.ProductImage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace ECommerce.API.Controllers
{
    [ApiController]
    public class ProductImageController : BaseController<ProductImageController>
    {
        private readonly IProductImageService _productImageService;

        public ProductImageController(ILogger<ProductImageController> logger, IProductImageService productImageService)
            : base(logger)
        {
            _productImageService = productImageService;
        }

        [HttpPost(ApiEndPointConstant.ProductImage.ProductImagesEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductImageResponse))]
        public async Task<IActionResult> Create([FromBody] ProductImageRequest request)
        {
            var response = await _productImageService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpDelete(ApiEndPointConstant.ProductImage.ProductImageEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productImageService.Delete(id);
            return Ok(result);
        }

        [HttpGet(ApiEndPointConstant.ProductImage.ProductImageEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductImageResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _productImageService.GetById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.ProductImage.ProductImagesEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<ProductImageResponse>))]
        public async Task<IActionResult> GetAll(string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _productImageService.GetAll(search, orderBy, page, size);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.ProductImage.ProductImagesByProductIdEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductImageResponse>))]
        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            var response = await _productImageService.GetByProductId(productId);
            return Ok(response);
        }

        [HttpPut(ApiEndPointConstant.ProductImage.SetDisplayOrderEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductImageResponse))]
        public async Task<IActionResult> SetDisplayOrder(Guid id, int displayOrder)
        {
            var response = await _productImageService.SetDisplayOrder(id, displayOrder);
            return Ok(response);
        }

        [HttpPut(ApiEndPointConstant.ProductImage.SetMainImageEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductImageResponse))]
        public async Task<IActionResult> SetMainImage(Guid id)
        {
            var response = await _productImageService.SetMainImage(id);
            return Ok(response);
        }

        //[HttpPut(ApiEndPointConstant.ProductImage.ProductImageEndpoint)]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductImageResponse))]
        //public async Task<IActionResult> Update(Guid id, [FromBody] ProductImageRequest request)
        //{
        //    var response = await _productImageService.Update(id, request);
        //    return Ok(response);
        //}
    }
}
