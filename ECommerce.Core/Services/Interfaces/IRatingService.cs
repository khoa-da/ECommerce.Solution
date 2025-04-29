using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Product;
using ECommerce.Shared.Payload.Request.Rating;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Shared.Payload.Response.Rating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IRatingService
    {
        Task<RatingResponse> Create(RatingRequest request);
        Task<RatingResponse> GetById(Guid id);
        Task<RatingResponse> Update(Guid id, RatingRequest request);
        Task<bool> Delete(Guid id);

        Task<IPaginate<RatingResponse>> GetAllByProductId(Guid id, string? search, string? orderBy, int page, int size);
        Task<IPaginate<RatingResponse>> GetAllByUserId(Guid id, string? search, string? orderBy, int page, int size);

    }
}
