using ECommerce.Shared.Payload.Request.Auth;
using ECommerce.Shared.Payload.Response.Auth;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task<LoginResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
    }
}
