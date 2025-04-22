using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class OrderController : BaseController<OrderController>
    {
        private readonly IOrderService _orderService;
        public OrderController(ILogger<OrderController> logger, IOrderService orderService) : base(logger)
        {
            _orderService = orderService;
        }
        //[HttpPost(ApiEndPointConstant.Order.OrdersEndpoint)]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderResponse))]
        //public async Task<IActionResult> Create([FromBody] OrderRequest request)
        //{
        //    var response = await _orderService.Create(request);
        //    return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        //}

        [HttpPost(ApiEndPointConstant.Order.OrdersEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderResponse))]
        public async Task<IActionResult> Create([FromBody] OrderRequest request)
        {
            var response = await _orderService.CreateV2(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
    }
}
