using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Payload.Request.StoreProduct;
using ECommerce.Shared.Payload.Response.StoreProduct;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class StoreProductController : BaseController<StoreProductController>
    {
        private readonly IStoreProductService _storeProductService;
        public StoreProductController(ILogger<StoreProductController> logger, IStoreProductService storeProductService) : base(logger)
        {
            _storeProductService = storeProductService;
        }

        [HttpPost(ApiEndPointConstant.StoreProduct.StoreProductsEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StoreProductResponse))]
        public async Task<IActionResult> Create([FromBody] StoreProductRequest request)
        {
            var response = await _storeProductService.CreateStoreProduct(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
        [HttpGet(ApiEndPointConstant.StoreProduct.StoreProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StoreProductDetailResponse))]
        public async Task<IActionResult> GetById(Guid storeId, Guid productId)
        {
            var response = await _storeProductService.GetStoreProductById(storeId, productId);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.StoreProduct.StoreProductsEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StoreProductResponse))]
        public async Task<IActionResult> Update(Guid id, [FromBody] StoreProductRequest request)
        {
            var response = await _storeProductService.UpdateStoreProduct(id, request);
            return Ok(response);
        }

        [HttpDelete(ApiEndPointConstant.StoreProduct.StoreProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> Delete(Guid storeId, Guid productId)
        {
            var response = await _storeProductService.DeleteStoreProduct(storeId, productId);
            return Ok(response);
        }


    }
}
