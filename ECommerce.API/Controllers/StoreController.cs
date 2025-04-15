﻿using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.Contants;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Store;
using ECommerce.Shared.Payload.Response.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    [ApiController]
    public class StoreController : BaseController<StoreController>
    {
        private readonly IStoreService _storeService;
        public StoreController(ILogger<StoreController> logger, IStoreService storeService) : base(logger)
        {
            _storeService = storeService;
        }
        [HttpPost(ApiEndPointConstant.Store.StoresEndpoint)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StoreResponse))]
        public async Task<IActionResult> Create([FromBody] StoreRequest request)
        {
            var response = await _storeService.Create(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
        [HttpGet(ApiEndPointConstant.Store.StoreEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StoreResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _storeService.GetById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Store.StoresEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginate<StoreResponse>))]
        public async Task<IActionResult> GetAll(string? search, string? orderBy, int page = 1, int size = 10)
        {
            var response = await _storeService.GetAllStore(search, orderBy, page, size);
            return Ok(response);
        }
        [HttpPut(ApiEndPointConstant.Store.StoreEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StoreResponse))]
        public async Task<IActionResult> Update(Guid id, [FromBody] StoreRequest request)
        {
            var response = await _storeService.Update(id, request);
            return Ok(response);
        }
    }
}
