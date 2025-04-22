using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.User;
using ECommerce.Shared.Payload.Response.User;

namespace ECommerce.Core.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserRequest, User>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();
        }
    }
}
