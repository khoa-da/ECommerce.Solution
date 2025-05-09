using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Infrastructure.Utils;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Auth;
using ECommerce.Shared.Payload.Response.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace ECommerce.Core.Services.Implementations
{
    public class AuthService : BaseService<AuthService>, IAuthService
    {
        private readonly ICartService _cartService;
        private readonly string _guestCartCookieName = "guest_cart_id";
        public AuthService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<AuthService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICartService cartService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cartService = cartService;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            // Tạo bộ lọc tìm kiếm theo username hoặc email
            Expression<Func<User, bool>> usernameOrEmailFilter = p =>
                p.Username.Equals(loginRequest.UsernameOrEmail) ||
                p.Email.Equals(loginRequest.UsernameOrEmail);

            // Lấy user từ cơ sở dữ liệu
            User existingUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: usernameOrEmailFilter);
            if (existingUser == null)
            {
                throw new EntityNotFoundException("User not found");
            }

            // Hash mật khẩu trước khi so sánh
            string hashedPassword = PasswordUtil.HashPassword(loginRequest.Password);

            // So sánh mật khẩu đã hash
            if (!existingUser.PasswordHash.Equals(hashedPassword))
            {
                throw new BusinessRuleException("Invalid Email or Username or Password");
            }

            // Tạo đối tượng LoginResponse
            LoginResponse loginResponse = new LoginResponse
            {
                UserId = existingUser.Id,
                Username = existingUser.Username,
                Email = existingUser.Email,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                PhoneNumber = existingUser.PhoneNumber,
                Status = existingUser.Status,
                Role = existingUser.Role,
            };

            // Tạo JWT token
            var token = JwtUtil.GenerateJwtToken(existingUser, "UserId", existingUser.Id);
            loginResponse.AccessToken = token.AccessToken;
            loginResponse.RefreshToken = token.RefreshToken;
            loginResponse.AccessTokenExpires = token.AccessTokenExpires;
            loginResponse.RefreshTokenExpires = token.RefreshTokenExpires;

            // Xử lý giỏ hàng (nếu có)
            try
            {
                string guestId = GetCookieValue(_guestCartCookieName);
                if (!string.IsNullOrEmpty(guestId))
                {
                    var guestCart = await _cartService.GetGuestCartAsync(guestId);
                    if (guestCart != null && guestCart.Items.Count > 0)
                    {
                        await _cartService.MergeCartsAsync(guestCart.Id, existingUser.Id);
                    }

                    DeleteCookie(_guestCartCookieName);
                }
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Error while merging guest cart with user cart");
            }

            return loginResponse;
        }


        public async Task<LoginResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(refreshTokenRequest.UserId));
            if (user == null) throw new EntityNotFoundException("User not found");
            var token = JwtUtil.RefreshToken(refreshTokenRequest);
            LoginResponse loginResponse = new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                Role = user.Role,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                AccessTokenExpires = token.AccessTokenExpires,
                RefreshTokenExpires = token.RefreshTokenExpires
            };

            return loginResponse;
        }
    }
}
