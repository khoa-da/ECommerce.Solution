using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Rating;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class RatingController : BaseController<RatingController>
    {
        private readonly IRatingService _ratingService;
        public RatingController(ILogger<RatingController> logger, IRatingService ratingService) : base(logger)
        {
            _ratingService = ratingService;
        }

        //[HttpGet(ApiEndPointConstant.Rating.RatingsEndpoint)]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<RatingResponse>))]
        //public async Task<IActionResult> GetAllByProductId(Guid id, string? search, string? orderBy, int page = 1, int size = 10)
        //{
        //    var response = await _ratingService.GetAllByProductId(id, search, orderBy, page, size);
        //    return Ok(response);
        //}
    

    }
}
