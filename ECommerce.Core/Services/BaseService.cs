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

        //protected string GetUsernameFromJwt()
        //{
        //    return _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //}

        //protected string GetRoleFromJwt()
        //{
        //    string role = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        //    return role;
        //}

        //protected string GetUserIdFromJwt()
        //{
        //    string userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    return userId;
        //}

    }
    
}
