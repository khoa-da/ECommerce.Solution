using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Rating;
using ECommerce.Shared.Payload.Response.Rating;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class RatingService : BaseService<RatingService>, IRatingService
    {
        public RatingService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<RatingService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<RatingResponse> Create(RatingRequest request)
        {
            // Kiểm tra sản phẩm tồn tại
            var productExists = await _unitOfWork.GetRepository<Product>().AnyAsync(
                predicate: p => p.Id == request.ProductId);

            if (!productExists)
            {
                throw new EntryPointNotFoundException("Product not found.");
            }

            // Kiểm tra người dùng tồn tại
            var userExists = await _unitOfWork.GetRepository<User>().AnyAsync(
                predicate: u => u.Id == request.UserId);

            if (!userExists)
            {
                throw new EntryPointNotFoundException("User not found.");
            }

            // Kiểm tra xem người dùng đã đánh giá sản phẩm này chưa
            var existingRating = await _unitOfWork.GetRepository<Rating>().AnyAsync(
                predicate: r => r.UserId == request.UserId && r.ProductId == request.ProductId);

            if (existingRating)
            {
                throw new DataConflictException("User has already rated this product.");
            }

            var rating = _mapper.Map<Rating>(request);
            rating.Id = Guid.NewGuid();
            rating.CreatedDate = DateTime.UtcNow.AddHours(7);

            await _unitOfWork.GetRepository<Rating>().InsertAsync(rating);

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to create rating.");
            }

            var response = _mapper.Map<RatingResponse>(rating);

            // Thêm thông tin người dùng và sản phẩm nếu cần
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: u => u.Id == rating.UserId);

            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: p => p.Id == rating.ProductId);

            if (user != null)
            {
                response.Email = user.Username;
            }

         

            return response;
        }

        public async Task<bool> Delete(Guid id)
        {
            var rating = await _unitOfWork.GetRepository<Rating>().SingleOrDefaultAsync(
                predicate: x => x.Id == id);

            if (rating == null)
            {
                throw new EntryPointNotFoundException("Rating not found");
            }

            _unitOfWork.GetRepository<Rating>().DeleteAsync(rating);

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IPaginate<RatingResponse>> GetAllByProductId(Guid productId, string? search, string? orderBy, int page, int size)
        {
            // Kiểm tra sản phẩm tồn tại
            var productExists = await _unitOfWork.GetRepository<Product>().AnyAsync(
                predicate: p => p.Id == productId);

            if (!productExists)
            {
                throw new EntryPointNotFoundException("Product not found.");
            }

            Expression<Func<Rating, bool>> predicate = x => x.ProductId == productId;

            if (!string.IsNullOrEmpty(search))
            {
                predicate = x => x.ProductId == productId && x.Comment != null && x.Comment.Contains(search);
            }

            Func<IQueryable<Rating>, IOrderedQueryable<Rating>> orderByFunc = null;

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "score":
                        orderByFunc = q => q.OrderBy(x => x.Score);
                        break;
                    case "score_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Score);
                        break;
                    case "date":
                        orderByFunc = q => q.OrderBy(x => x.CreatedDate);
                        break;
                    case "date_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                    default:
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            else
            {
                orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
            }

            var ratings = await _unitOfWork.GetRepository<Rating>().GetPagingListAsync(
                selector: x => _mapper.Map<RatingResponse>(x),
                predicate: predicate,
                orderBy: orderByFunc,
                page: page,
                size: size
            );

            // Bổ sung thông tin người dùng cho mỗi đánh giá
            foreach (var rating in ratings.Items)
            {
                var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    predicate: u => u.Id == rating.UserId);

                if (user != null)
                {
                    rating.Email = user.Username;
                }

               
            }

            return ratings;
        }

        public async Task<IPaginate<RatingResponse>> GetAllByUserId(Guid userId, string? search, string? orderBy, int page, int size)
        {
            // Kiểm tra người dùng tồn tại
            var userExists = await _unitOfWork.GetRepository<User>().AnyAsync(
                predicate: u => u.Id == userId);

            if (!userExists)
            {
                throw new EntryPointNotFoundException("User not found.");
            }

            Expression<Func<Rating, bool>> predicate = x => x.UserId == userId;

            if (!string.IsNullOrEmpty(search))
            {
                predicate = x => x.UserId == userId && x.Comment != null && x.Comment.Contains(search);
            }

            Func<IQueryable<Rating>, IOrderedQueryable<Rating>> orderByFunc = null;

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "score":
                        orderByFunc = q => q.OrderBy(x => x.Score);
                        break;
                    case "score_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.Score);
                        break;
                    case "date":
                        orderByFunc = q => q.OrderBy(x => x.CreatedDate);
                        break;
                    case "date_desc":
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                    default:
                        orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
                        break;
                }
            }
            else
            {
                orderByFunc = q => q.OrderByDescending(x => x.CreatedDate);
            }

            var ratings = await _unitOfWork.GetRepository<Rating>().GetPagingListAsync(
                selector: x => _mapper.Map<RatingResponse>(x),
                predicate: predicate,
                orderBy: orderByFunc,
                page: page,
                size: size
            );

            // Bổ sung thông tin sản phẩm cho mỗi đánh giá
            foreach (var rating in ratings.Items)
            {
                rating.Email = (await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    predicate: u => u.Id == rating.UserId))?.Email;

              
            }

            return ratings;
        }

        public async Task<RatingResponse> GetById(Guid id)
        {
            var rating = await _unitOfWork.GetRepository<Rating>().SingleOrDefaultAsync(
                predicate: x => x.Id == id
            );

            if (rating == null)
            {
                throw new EntryPointNotFoundException("Rating not found");
            }

            var response = _mapper.Map<RatingResponse>(rating);

            // Bổ sung thông tin người dùng và sản phẩm
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: u => u.Id == rating.UserId);

            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: p => p.Id == rating.ProductId);

            if (user != null)
            {
                response.Email = user.Email;
            }

           

            return response;
        }

        public async Task<RatingResponse> Update(Guid id, RatingRequest request)
        {
            var rating = await _unitOfWork.GetRepository<Rating>().SingleOrDefaultAsync(
                predicate: x => x.Id == id);

            if (rating == null)
            {
                throw new EntryPointNotFoundException("Rating not found");
            }

            // Kiểm tra xem người dùng có quyền cập nhật không
            if (rating.UserId != request.UserId)
            {
                throw new BusinessRuleException("User cannot update ratings from other users");
            }

            // Cập nhật thông tin đánh giá
            rating.Score = request.Score;
            rating.Comment = request.Comment;

            _unitOfWork.GetRepository<Rating>().UpdateAsync(rating);

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new DataConflictException("Failed to update rating");
            }

            var ratingResponse = _mapper.Map<RatingResponse>(rating);

            // Bổ sung thông tin người dùng và sản phẩm
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: u => u.Id == rating.UserId);

            var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: p => p.Id == rating.ProductId);

            if (user != null)
            {
                ratingResponse.Email = user.Email;
            }


            return ratingResponse;
        }
    }
}