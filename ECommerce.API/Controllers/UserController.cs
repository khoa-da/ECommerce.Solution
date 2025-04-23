using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.User;
using ECommerce.Shared.Payload.Response.Order;
using ECommerce.Shared.Payload.Response.User;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class UserController : BaseController<UserController>
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public UserController(ILogger<UserController> logger, IUserService userService, IOrderService oderService) : base(logger)
        {
            _userService = userService;
            _orderService = oderService;
        }

        [HttpPost(ApiEndPointConstant.User.UsersEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponse))]
        public async Task<IActionResult> CreateNewUser([FromBody] UserRequest createUserRequest)
        {
            var userResponse = await _userService.CreateUser(createUserRequest);
            return CreatedAtAction(nameof(CreateNewUser), new { id = userResponse.Id }, userResponse);
        }

        [HttpGet(ApiEndPointConstant.User.UserEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var userResponse = await _userService.GetById(id);
            return Ok(userResponse);
        }

        [HttpGet(ApiEndPointConstant.User.UserByFieldEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        public async Task<IActionResult> GetUserByField(string field, string value)
        {
            var userResponse = await _userService.GetByField(field, value);
            return Ok(userResponse);
        }


        [HttpGet(ApiEndPointConstant.User.UsersEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<UserResponse>))]
        public async Task<IActionResult> GetAllUsers(string? search, string? orderBy, int page = 1, int size = 10)
        {
            var userResponse = await _userService.GetAll(search, orderBy, page, size);
            return Ok(userResponse);
        }

        [HttpGet(ApiEndPointConstant.User.OrderByUserIdEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<OrderResponse>))]
        public async Task<IActionResult> GetOrdersByUserId(Guid id, string? search, string? orderBy, int page = 1, int size = 10)
        {
            var orderResponse = await _orderService.GetAllByUserId(id, search, orderBy, page, size);
            return Ok(orderResponse);

        }
    }
}
