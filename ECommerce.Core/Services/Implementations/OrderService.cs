using AutoMapper;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Implementations
{
    public class OrderService : BaseService<OrderService>, IOrderService
    {
        public OrderService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<OrderService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public Task<OrderResponse> ChangeStatus(Guid id, string status)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> Create(OrderRequest order)
        {
            // Validate the order request
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order request cannot be null.");
            }
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Id == order.UserId);
            if(user == null)
            {
                throw new EntryPointNotFoundException("User not found.");
            }
            var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(predicate: x => x.Id == order.StoreId);
            if (store == null)
            {
                throw new EntryPointNotFoundException("Store not found.");
            }

            var orderEntity = _mapper.Map<Order>(order);
            orderEntity.Id = Guid.NewGuid();
            orderEntity.OrderDate = DateTime.UtcNow.AddHours(7);
            orderEntity.OrderNumber = GenerateOrderNumber(store.Name);
            orderEntity.PaymentStatus = OrderEnum.PaymentStatus.Pending.ToString();
            orderEntity.OrderStatus = OrderEnum.OrderStatus.Processing.ToString();

            //Process the order items
            if(order.ProductId != null && order.ProductId.Count > 0)
            {
                var orderItems = new List<OrderItem>();
                foreach (var itemId in order.ProductId)
                {
                    var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: x => x.Id == itemId);
                    if (product == null)
                    {
                        throw new EntryPointNotFoundException($"Product with ID {itemId} not found.");
                    }
                    var orderItemEntity = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderEntity.Id,
                        ProductId = product.Id,
                        //Quantity = orderItem.Quantity,
                        Price = product.Price,
                        //TotalAmount = product.Price * orderItem.Quantity
                    };

                    orderItems.Add(orderItemEntity);

                }
                orderEntity.OrderItems = orderItems;
                orderEntity.TotalAmount = orderItems.Sum(item => item.TotalAmount);
            }

            await _unitOfWork.GetRepository<Order>().InsertAsync(orderEntity);
            if(await _unitOfWork.CommitAsync() <= 0)
            {
                throw new Exception("Failed to create order.");
            }

            var orderResponse = _mapper.Map<OrderResponse>(orderEntity);
            orderResponse.FirstName = user.FirstName;
            orderResponse.LastName = user.LastName;
            orderResponse.Email = user.Email;
            orderResponse.PhoneNumber = user.PhoneNumber;
            orderResponse.StoreName = store.Name;
            orderResponse.StorePhoneNumber = store.PhoneNumber;
            return orderResponse;
        }
        private string GenerateOrderNumber(string storeName)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var orderNumber = $"{storeName}-{randomString}";
            return orderNumber;
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginate<OrderResponse>> GetAll(string? search, string? orderBy, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginate<OrderResponse>> GetByUserId(Guid userId, string? search, string? orderBy, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse> Update(Guid id, OrderRequest order)
        {
            throw new NotImplementedException();
        }
    }
}
