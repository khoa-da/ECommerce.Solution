using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Category;
using ECommerce.Shared.Payload.Response.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
