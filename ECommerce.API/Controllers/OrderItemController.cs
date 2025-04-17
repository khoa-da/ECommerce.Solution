using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Payload.Response.OrderItem;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    public class OrderItemController : BaseController<OrderItemController>
    {
        private readonly IOrderItemService _orderItemService;
        public OrderItemController(ILogger<OrderItemController> logger, IOrderItemService orderItemService) : base(logger)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet(ApiEndPointConstant.OrderItem.OrderItemEndpoint)]
        [ProducesResponseType(typeof(OrderItemResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderItemById(Guid id)
        {
            var orderItem = await _orderItemService.GetById(id);
            return Ok(orderItem);
        }

    }
}
