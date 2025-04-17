using AutoMapper;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace ECommerce.Core.Services
{
    public abstract class BaseService<T> where T : class
    {
        protected IUnitOfWork<EcommerceDbContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        public BaseService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<T> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetUsernameFromJwt()
        {
            return _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        protected string GetRoleFromJwt()
        {
            string role = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            return role;
        }

        protected string GetUserIdFromJwt()
        {
            string userId = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }

        protected string GetCookieValue(string cookieName)
        {
            string value = null;
            _httpContextAccessor?.HttpContext?.Request.Cookies.TryGetValue(cookieName, out value);
            return value;
        }
        protected void SetCookie(string cookieName, string value, TimeSpan expiry)
        {
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
                
                cookieName,
                value,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(7).Add(expiry),
                    HttpOnly = true,
                    Secure = _httpContextAccessor.HttpContext.Request.IsHttps,
                    SameSite = SameSiteMode.Lax
                }
            );
        }
        protected bool IsSecureConnection()
        {
            return _httpContextAccessor?.HttpContext?.Request.IsHttps ?? false;
        }

        protected string GenerateNewGuid()
        {
            return Guid.NewGuid().ToString();
        }
        protected void DeleteCookie(string cookieName)
        {
            _httpContextAccessor?.HttpContext?.Response.Cookies.Delete(cookieName);
        }


    }
    
}
