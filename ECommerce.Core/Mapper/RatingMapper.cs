using AutoMapper;
using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Rating;
using ECommerce.Shared.Payload.Response.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Mapper
{
    public class RatingMapper : Profile
    {
        public RatingMapper()
        {
            CreateMap<RatingRequest, Rating>();
            CreateMap<Rating, RatingResponse>();
        }
    }
}
