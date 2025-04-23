using AutoMapper;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;
using ECommerce.Shared.Payload.Response.OrderItem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;


namespace ECommerce.Core.Services.Implementations
{
    public class OrderService : BaseService<OrderService>, IOrderService
    {
        private readonly ICartService _cartService;
        public OrderService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<OrderService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICartService cartService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _cartService = cartService;
        }

        public Task<OrderResponse> ChangeStatus(Guid id, string status)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse> Create(OrderRequest order)
        {
            //// Validate the order request
            //if (order == null)
            //{
            //    throw new ArgumentNullException(nameof(order), "Order request cannot be null.");
            //}

            //// Validate user
            //var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Id == order.UserId);
            //if (user == null)
            //{
            //    throw new EntryPointNotFoundException("User not found.");
            //}

            //// Validate store
            //var store = await _unitOfWork.GetRepository<Store>().SingleOrDefaultAsync(predicate: x => x.Id == order.StoreId);
            //if (store == null)
            //{
            //    throw new EntryPointNotFoundException("Store not found.");
            //}

            //// Get user's cart
            //var cart = await _cartService.GetUserCartAsync(order.UserId);
            //if (cart == null || cart.Items.Count == 0)
            //{
            //    throw new InvalidOperationException("Cart is empty. Cannot create order.");
            //}

            //// Create order entity
            //var orderEntity = _mapper.Map<Order>(order);
            //orderEntity.Id = Guid.NewGuid();
            //orderEntity.OrderDate = DateTime.UtcNow.AddHours(7);
            //orderEntity.OrderNumber = GenerateOrderNumber(store.Name);
            //orderEntity.PaymentStatus = OrderEnum.PaymentStatus.Pending.ToString();
            //orderEntity.OrderStatus = OrderEnum.OrderStatus.Processing.ToString();

            //// Process the order items from cart
            //var orderItems = new List<OrderItem>();
            //foreach (var cartItem in cart.Items)
            //{
            //    // Verify product still exists and stock is available
            //    var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            //        predicate: x => x.Id == cartItem.ProductId);

            //    if (product == null)
            //    {
            //        throw new EntryPointNotFoundException($"Product with ID {cartItem.ProductId} not found.");
            //    }

            //    // Check stock availability
            //    var storeProduct = await _unitOfWork.GetRepository<StoreProduct>()
            //        .SingleOrDefaultAsync(predicate: x => x.ProductId == cartItem.ProductId && x.StoreId == order.StoreId);

            //    if (storeProduct == null)
            //    {
            //        throw new InvalidOperationException($"Product {product.Name} is not available in this store.");
            //    }

            //    if (storeProduct.Stock < cartItem.Quantity)
            //    {
            //        throw new InvalidOperationException($"Not enough stock for product {product.Name}. Available: {storeProduct.Stock}, Requested: {cartItem.Quantity}");
            //    }

            //    // Create order item
            //    var orderItemEntity = new OrderItem
            //    {
            //        Id = Guid.NewGuid(),
            //        OrderId = orderEntity.Id,
            //        ProductId = cartItem.ProductId,
            //        Quantity = cartItem.Quantity,
            //        Price = cartItem.Price,
            //        TotalAmount = cartItem.Quantity * cartItem.Price
            //    };

            //    orderItems.Add(orderItemEntity);

            //    // Update product stock
            //    storeProduct.Stock -= cartItem.Quantity;
            //    _unitOfWork.GetRepository<StoreProduct>().UpdateAsync(storeProduct);
            //}

            //orderEntity.OrderItems = orderItems;
            //orderEntity.TotalAmount = orderItems.Sum(item => item.TotalAmount);

            //// Save the order
            //await _unitOfWork.GetRepository<Order>().InsertAsync(orderEntity);

            //// Clear the cart after successful order creation
            //await _cartService.DeleteCartAsync(cart.Id);

            //if (await _unitOfWork.CommitAsync() <= 0)
            //{
            //    throw new Exception("Failed to create order.");
            //}

            //// Map response
            //var orderResponse = _mapper.Map<OrderResponse>(orderEntity);
            //orderResponse.FirstName = user.FirstName;
            //orderResponse.LastName = user.LastName;
            //orderResponse.Email = user.Email;
            //orderResponse.PhoneNumber = user.PhoneNumber;
            //orderResponse.StoreName = store.Name;
            //orderResponse.StorePhoneNumber = store.PhoneNumber;

            //return orderResponse;
            throw new NotImplementedException();
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

        public async Task<OrderResponse> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "Order ID cannot be empty.");
            }
            var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: x => x.Id == id,
                include: x => x.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Include(u => u.User).Include(s => s.Store));
            if (order == null)
            {
                throw new EntityNotFoundException("Order not found.");
            }
            var orderResponse = _mapper.Map<OrderResponse>(order);
            orderResponse.FirstName = order.User.FirstName;
            orderResponse.LastName = order.User.LastName;
            orderResponse.Email = order.User.Email;
            orderResponse.PhoneNumber = order.User.PhoneNumber;
            orderResponse.StoreName = order.Store.Name;
            orderResponse.StorePhoneNumber = order.Store.PhoneNumber;
            orderResponse.OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name,
                CategoryId = oi.Product.CategoryId,
                CategoryName = oi.Product.Name,
                Gender = oi.Product.Gender,
                Size = oi.Product.Size,
                Brand = oi.Product.Brand,
                Sku = oi.Product.Sku,
                Tags = oi.Product.Tags,
                Material = oi.Product.Material,
                Quantity = oi.Quantity,
                Price = oi.Price,
                TotalAmount = oi.TotalAmount
            }).ToList();
            
            return orderResponse;
        }

        public Task<IPaginate<OrderResponse>> GetByUserId(Guid userId, string? search, string? orderBy, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse> Update(Guid id, OrderRequest order)
        {
            throw new NotImplementedException();
        }

      

        // Method to clear cart from cookies
        private void ClearCartCookies()
        {
            const string CartCookieKey = "Cart";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CartCookieKey);
        }

        public async Task<OrderResponse> CreateV2(OrderRequest order)
        {
            // Validate the order request
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order request cannot be null.");
            }
            if (order.UserId == Guid.Empty)
            {
                order.UserId = Guid.Parse(GetUserIdFromJwt());
            }
            // Validate user
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Id == order.UserId);
            if (user == null)
            {
                throw new EntryPointNotFoundException("User not found.");
            }

            // Get cart from Cookies instead of Session
            //var cartItems = GetCartItemsFromCookies();
            var cartItems = order.CartItems;
            if (cartItems == null || cartItems.Count == 0)
            {
                throw new InvalidOperationException("Cart is empty. Cannot create order.");
            }
            if (cartItems == null || cartItems.Count == 0)
            {
                throw new InvalidOperationException("Cart is empty. Cannot create order.");
            }

            // Create order entity
            var orderEntity = _mapper.Map<Order>(order);
            orderEntity.Id = Guid.NewGuid();
            orderEntity.OrderDate = DateTime.UtcNow.AddHours(7);

            // Set hard-coded store info
            Guid hardCodedStoreId = Guid.Parse("98ECB2CF-F3D0-47D5-B3B9-3F08B6921FC1"); // Replace with your actual store ID
            string storeName = "Main Store"; // Replace with your actual store name
            string storePhone = "1234567890"; // Replace with your actual store phone

            orderEntity.StoreId = hardCodedStoreId;
            orderEntity.OrderNumber = GenerateOrderNumber(storeName);
            orderEntity.PaymentStatus = OrderEnum.PaymentStatus.Pending.ToString();
            orderEntity.OrderStatus = OrderEnum.OrderStatus.Processing.ToString();

            // Process the order items from cart in cookies
            var orderItems = new List<OrderItem>();
            var orderItemResponses = new List<OrderItemResponse>();
            foreach (var cartItem in cartItems)
            {
                // Verify product still exists and check stock directly in Product
                var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                    predicate: x => x.Id == cartItem.ProductId);

                if (product == null)
                {
                    throw new EntryPointNotFoundException($"Product with ID {cartItem.ProductId} not found.");
                }

                // Check stock availability directly in Product
                if (product.Stock < cartItem.Quantity)
                {
                    throw new InvalidOperationException($"Not enough stock for product {product.Name}. Available: {product.Stock}, Requested: {cartItem.Quantity}");
                }

                // Create order item
                var orderItemEntity = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderEntity.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                    TotalAmount = cartItem.Quantity * cartItem.Price


                };

                orderItems.Add(orderItemEntity);

                // Update product stock directly
                product.Stock -= cartItem.Quantity;
                _unitOfWork.GetRepository<Product>().UpdateAsync(product);

                var orderItemResponse = new OrderItemResponse
                {
                    Id = orderItemEntity.Id,
                    OrderId = orderEntity.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name, // nếu có navigation
                    Gender = product.Gender,
                    Size = product.Size,
                    Brand = product.Brand,
                    Sku = product.Sku,
                    Tags = product.Tags,
                    Material = product.Material,
                    Quantity = orderItemEntity.Quantity,
                    Price = orderItemEntity.Price,
                    TotalAmount = orderItemEntity.TotalAmount
                };
                orderItemResponses.Add(orderItemResponse);
            }

            orderEntity.OrderItems = orderItems;
            orderEntity.TotalAmount = orderItems.Sum(item => item.TotalAmount);

            // Save the order
            await _unitOfWork.GetRepository<Order>().InsertAsync(orderEntity);

            // Clear the cart after successful order creation
            ClearCartCookies();

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                throw new Exception("Failed to create order.");
            }

            // Map response
            var orderResponse = _mapper.Map<OrderResponse>(orderEntity);
            orderResponse.FirstName = user.FirstName;
            orderResponse.LastName = user.LastName;
            orderResponse.Email = user.Email;
            orderResponse.PhoneNumber = user.PhoneNumber;
            orderResponse.StoreName = storeName;
            orderResponse.StorePhoneNumber = storePhone;
            orderResponse.OrderItems = orderItemResponses;


            return orderResponse;
        }

      

        public async Task<IPaginate<OrderResponse>> GetAllByUserId(Guid userId, string? search, string? orderBy, int page, int size)
        {
            //Validate userId
            if(userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be empty.");
            }
            
            //Find user by userId
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Id == userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found.");
            }
            search = search?.Trim().ToLower();
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderByFunc = x => x.OrderByDescending(o => o.OrderDate);
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "order_date_asc":
                        orderByFunc = x => x.OrderBy(o => o.OrderDate);
                        break;
                    case "order_date_desc":
                        orderByFunc = x => x.OrderByDescending(o => o.OrderDate);
                        break;
                    case "total_amount_asc":
                        orderByFunc = x => x.OrderBy(o => o.TotalAmount);
                        break;
                    case "total_amount_desc":
                        orderByFunc = x => x.OrderByDescending(o => o.TotalAmount);
                        break;
                }
            }
            var orderItemResponse = new List<OrderItemResponse>();
            var orders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                selector: x => new OrderResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    PhoneNumber = x.User.PhoneNumber,
                    StoreId = x.StoreId,
                    StoreName = x.Store.Name,
                    StorePhoneNumber = x.Store.PhoneNumber,
                    OrderNumber = x.OrderNumber,
                    OrderDate = x.OrderDate,
                    TotalAmount = x.TotalAmount,
                    PaymentStatus = x.PaymentStatus,
                    OrderStatus = x.OrderStatus,
                    ShippingAddress = x.ShippingAddress,
                    PaymentMethod = x.PaymentMethod,
                    ShippingMethod = x.ShippingMethod,
                    OrderItems = x.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        CategoryId = oi.Product.CategoryId,
                        CategoryName = oi.Product.Name,
                        Gender = oi.Product.Gender,
                        Size = oi.Product.Size,
                        Brand = oi.Product.Brand,
                        Sku = oi.Product.Sku,
                        Tags = oi.Product.Tags,
                        Material = oi.Product.Material,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        TotalAmount = oi.TotalAmount
                    }).ToList(),
                    Notes = x.Notes
                },
                predicate: x => x.UserId == userId && (string.IsNullOrEmpty(search) || x.OrderNumber.ToLower().Contains(search)),
                orderBy: orderByFunc,
                include: x => x.Include(o => o.OrderItems).ThenInclude(oi => oi.Product),
                page: page,
                size: size
                );

            return orders;
        }


    }
}
