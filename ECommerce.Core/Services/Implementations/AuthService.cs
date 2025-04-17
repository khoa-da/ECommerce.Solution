using AutoMapper;
using Azure.Messaging;
using ECommerce.Infrastructure.Utils;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Auth;
using ECommerce.Shared.Payload.Response.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            Expression<Func<User, bool>> usernameOrEmailFilter = p => p.Username.Equals(loginRequest.UsernameOrEmail) || p.Email.Equals(loginRequest.UsernameOrEmail);
            User existingUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: usernameOrEmailFilter);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            Expression<Func<User, bool>> searchFilter = p => (p.Username.Equals(loginRequest.UsernameOrEmail) || p.Email.Equals(loginRequest.UsernameOrEmail)) && p.PasswordHash.Equals(PasswordUtil.HashPassword(loginRequest.Password));

            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: searchFilter);
            if (user == null)
            {
                throw new Exception("Invalid Email or Username or Password");
            }
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
            };
            // Generate JWT token
            var token = JwtUtil.GenerateJwtToken(user, "UserId", user.Id);
            loginResponse.AccessToken = token.AccessToken;
            loginResponse.RefreshToken = token.RefreshToken;
            loginResponse.AccessTokenExpires = token.AccessTokenExpires;
            loginResponse.RefreshTokenExpires = token.RefreshTokenExpires;


            try
            {
                string guestId = GetCookieValue(_guestCartCookieName);
                if (!string.IsNullOrEmpty(guestId))
                {
                    var guestCart = await _cartService.GetGuestCartAsync(guestId);
                    if (guestCart != null && guestCart.Items.Count > 0)
                    {
                        // Merge guest cart with user cart
                        await _cartService.MergeCartsAsync(guestCart.Id, user.Id);
                    }

                    // Delete guest cart cookie
                    DeleteCookie(_guestCartCookieName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while merging guest cart with user cart");
            }

            return loginResponse;
            
        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(refreshTokenRequest.UserId));
            if (user == null) throw new Exception("User not found");
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
