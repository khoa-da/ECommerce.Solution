using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.User;
using ECommerce.Shared.Payload.Response.User;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserResponse> CreateUser(UserRequest createUserRequest);
        public Task<UserResponse> GetByField(string field, string value);
        public Task<UserResponse> GetById(Guid id);

        public Task<IPaginate<UserResponse>> GetAll(string? search, string? orderBy, int page, int size);
        //public Task<UserResponse> UpdateUser(Guid id, UserRequest updateUserRequest);

    }
}
