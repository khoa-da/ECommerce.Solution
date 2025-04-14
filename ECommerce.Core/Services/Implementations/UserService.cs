using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Infrastructure.Utils;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.User;
using ECommerce.Shared.Payload.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class UserService : BaseService<UserService>, IUserService
    {
        public UserService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<UserResponse> CreateUser(UserRequest createUserRequest)
        {
            // Validate the request
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.Username == createUserRequest.Username || u.Email == createUserRequest.Email);
            if (user != null)
            {
                throw new EntityNotFoundException("User with this username or email already exists.");
            }
            var existingPhoneNumber = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.PhoneNumber == createUserRequest.PhoneNumber);
            if (existingPhoneNumber != null)
            {
                throw new EntityNotFoundException("User with this phone number already exists.");
            }
            var newUser = _mapper.Map<User>(createUserRequest);
            newUser.Id = Guid.NewGuid();
            newUser.PasswordHash = PasswordUtil.HashPassword(createUserRequest.PasswordHash);
            newUser.Role = RoleEnum.Customer.ToString();
            newUser.RegisteredDate = DateTime.UtcNow.AddHours(7);
            newUser.Status = UserEnum.Status.Active.ToString();

            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            if(await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to create user.");
            }
            var userResponse = _mapper.Map<UserResponse>(newUser);
            return userResponse;
        }

        public async Task<IPaginate<UserResponse>> GetAll(string? search, string? orderBy, int page, int size)
        {
            search = search?.Trim().ToLower();

            Func<IQueryable<User>, IOrderedQueryable<User>> orderByFunc = users => users.OrderByDescending(u => u.RegisteredDate); 

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "first_name_asc":
                        orderByFunc = users => users.OrderBy(u => u.FirstName);
                        break;
                    case "first_name_desc":
                        orderByFunc = users => users.OrderByDescending(u => u.FirstName);
                        break;
                    case "last_name_asc":
                        orderByFunc = users => users.OrderBy(u => u.LastName);
                        break;
                    case "last_name_desc":
                        orderByFunc = users => users.OrderByDescending(u => u.LastName);
                        break;
                    case "status_asc":
                        orderByFunc = users => users.OrderBy(u => u.Status);
                        break;
                    case "status_desc":
                        orderByFunc = users => users.OrderByDescending(u => u.Status);
                        break;
                }
            }

            var user = await _unitOfWork.GetRepository<User>().GetPagingListAsync(
                selector: x => _mapper.Map<UserResponse>(x),
                predicate: string.IsNullOrEmpty(search) ? x => true : x => x.PhoneNumber.ToLower().Contains(search),
                orderBy: orderByFunc,
                page: page,
                size: size);

            return user;
        }

        public async Task<UserResponse> GetByField(string field, string value)
        {
            User user = null;
            switch (field.ToLower())
            {
                case "id":
                    if (!Guid.TryParse(value, out Guid id))
                    {
                        throw new ArgumentException("Invalid ID format.");
                    }
                    user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.Id == id);
                    break;
                case "user_name":
                    user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.Username == value);
                    break;
                case "email":
                    user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.Email == value);
                    break;
                case "phone_number":
                    user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.PhoneNumber == value);
                    break;
                case "first_name":
                    user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.FirstName == value);
                    break;
                case "last_name":
                    user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.LastName == value);
                    break;
                default:
                    throw new ArgumentException("Invalid field specified.");
            }
            if (user == null)
            {
                throw new EntityNotFoundException($"User with {field} '{value}' not found.");
            }

            var userResponse = _mapper.Map<UserResponse>(user);
            return userResponse;
        }

        public async Task<UserResponse> GetById(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be empty.");
            }
           
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: u => u.Id == id);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found.");
            }
            var userResponse = _mapper.Map<UserResponse>(user);
            return userResponse;
        }
    }
}
