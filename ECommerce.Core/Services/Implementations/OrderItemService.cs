using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.OrderItem;
using ECommerce.Shared.Payload.Response.OrderItem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core.Services.Implementations
{
    public class OrderItemService : BaseService<OrderItemService>, IOrderItemService
    {
        public OrderItemService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<OrderItemService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public Task<OrderItemResponse> Create(OrderItemRequest orderItem)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginate<OrderItemResponse>> GetAll(string? search, string? orderBy, int page, int size)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderItemResponse> GetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id), "Id cannot be empty.");
            var orderItem = await _unitOfWork.GetRepository<OrderItem>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (orderItem == null)
                throw new EntityNotFoundException($"OrderItem with id {id} not found.");
            var orderItemResponse = _mapper.Map<OrderItemResponse>(orderItem);
            orderItemResponse.ProductName = orderItem.Product.Name;
            orderItemResponse.CategoryId = orderItem.Product.CategoryId;
            orderItemResponse.CategoryName = orderItem.Product.Category.Name;
            orderItemResponse.Gender = orderItem.Product.Gender;
            orderItemResponse.Size = orderItem.Product.Size;
            orderItemResponse.Brand = orderItem.Product.Brand;
            orderItemResponse.Sku = orderItem.Product.Sku;
            orderItemResponse.Tags = orderItem.Product.Tags;
            orderItemResponse.Material = orderItem.Product.Material;

            return orderItemResponse;

        }

        public async Task<IPaginate<OrderItemResponse>> GetByOrderId(Guid orderId, string? search, string? orderBy, int page, int size)
        {
            search = search?.Trim().ToLower();

            Func<IQueryable<OrderItem>, IOrderedQueryable<OrderItem>> orderByFunc = x => x.OrderByDescending(x => x.Quantity);
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "quantity":
                        orderByFunc = x => x.OrderBy(x => x.Quantity);
                        break;
                    case "quantity_desc":
                        orderByFunc = x => x.OrderByDescending(x => x.Quantity);
                        break;
                    case "price":
                        orderByFunc = x => x.OrderBy(x => x.Price);
                        break;
                    case "price_desc":
                        orderByFunc = x => x.OrderByDescending(x => x.Price);
                        break;
                    default:
                        orderByFunc = x => x.OrderByDescending(x => x.Quantity);
                        break;
                }
            }


            if (orderId == Guid.Empty)
                throw new ArgumentNullException(nameof(orderId), "OrderId cannot be empty.");

            var orderItems = await _unitOfWork.GetRepository<OrderItem>().GetPagingListAsync(
                selector: x => new OrderItemResponse
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    CategoryId = x.Product.CategoryId,
                    CategoryName = x.Product.Category.Name,
                    Gender = x.Product.Gender,
                    Size = x.Product.Size,
                    Brand = x.Product.Brand,
                    Sku = x.Product.Sku,
                    Tags = x.Product.Tags,
                    Material = x.Product.Material,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TotalAmount = x.TotalAmount
                },
                predicate: x => x.OrderId == orderId,
                orderBy: orderByFunc,
                include: x => x.Include(x => x.Product).Include(x => x.Product.Category),
                page: page,
                size: size
                );
            if (orderItems == null)
                throw new EntityNotFoundException($"OrderItem with orderId {orderId} not found.");
            var orderItemResponse = _mapper.Map<IPaginate<OrderItemResponse>>(orderItems);
            return orderItemResponse;

        }

        public Task<OrderItemResponse> Update(Guid id, OrderItemRequest orderItem)
        {
            throw new NotImplementedException();
        }
    }
}
