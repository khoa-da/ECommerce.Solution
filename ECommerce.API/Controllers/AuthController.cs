using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Payload.Request.Auth;
using ECommerce.Shared.Payload.Response.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class AuthController : BaseController<AuthController>
    {
        private readonly IAuthService _authService;
        public AuthController(ILogger<AuthController> logger, IAuthService authService) : base(logger)
        {
            _authService = authService;
        }
        [HttpPost(ApiEndPointConstant.Authentication.Login)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var loginResponse = await _authService.Login(loginRequest);
            return Ok(loginResponse);
        }

        [HttpPost(ApiEndPointConstant.Authentication.RefreshToken)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var loginResponse = await _authService.RefreshToken(refreshTokenRequest);
            return Ok(loginResponse);
        }

    }
}
