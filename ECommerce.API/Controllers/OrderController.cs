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
        [HttpGet(ApiEndPointConstant.Order.OrderEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _orderService.GetById(id);
            return Ok(response);
        }

        [HttpPut(ApiEndPointConstant.Order.CancelOrderEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponse))]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest cancelOrderRequest)
        {
            var response = await _orderService.CancelOrder(cancelOrderRequest.OrderId, cancelOrderRequest.Reason);
            return Ok(response);
        }
    }
}
