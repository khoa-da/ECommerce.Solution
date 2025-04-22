using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Category;
using ECommerce.Shared.Payload.Response.Category;

namespace ECommerce.Core.Mapper
{
    public class CategoryMapper : Profile
    {
        public CategoryMapper()
        {
            CreateMap<Category, CategoryResponse>()
                .ReverseMap();
            CreateMap<CategoryRequest, Category>()
                .ReverseMap();

        }
    }
}
