using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Rating;
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

        [HttpPost(ApiEndPointConstant.Rating.RatingsEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RatingResponse))]
        public async Task<IActionResult> Create([FromBody] RatingRequest request)
        {
            var response = await _ratingService.Create(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }

        [HttpGet(ApiEndPointConstant.Rating.RatingEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RatingResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _ratingService.GetById(id);
            return Ok(response);
        }




    }
}
