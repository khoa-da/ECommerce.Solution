using AutoMapper;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Models;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

        public async Task<OrderResponse> CreateV2(OrderRequest order)
        {
            // Validate the order request
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order request cannot be null.");
            }
            if(order.UserId == Guid.Empty)
            {
                order.UserId = Guid.Parse(GetUserIdFromJwt());
            }
            // Validate user
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Id == order.UserId);
            if (user == null)
            {
                throw new EntryPointNotFoundException("User not found.");
            }

            // Get cart from Session
            var cartItems = GetCartItemsFromSession();
            if (cartItems == null || cartItems.Count == 0)
            {
                throw new InvalidOperationException("Cart is empty. Cannot create order.");
            }

            // Create order entity
            var orderEntity = _mapper.Map<Order>(order);
            orderEntity.Id = Guid.NewGuid();
            orderEntity.OrderDate = DateTime.UtcNow.AddHours(7);

            // Set hard-coded store info
            Guid hardCodedStoreId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Replace with your actual store ID
            string storeName = "Main Store"; // Replace with your actual store name
            string storePhone = "1234567890"; // Replace with your actual store phone

            orderEntity.StoreId = hardCodedStoreId;
            orderEntity.OrderNumber = GenerateOrderNumber(storeName);
            orderEntity.PaymentStatus = OrderEnum.PaymentStatus.Pending.ToString();
            orderEntity.OrderStatus = OrderEnum.OrderStatus.Processing.ToString();

            // Process the order items from cart in session
            var orderItems = new List<OrderItem>();
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
            }

            orderEntity.OrderItems = orderItems;
            orderEntity.TotalAmount = orderItems.Sum(item => item.TotalAmount);

            // Save the order
            await _unitOfWork.GetRepository<Order>().InsertAsync(orderEntity);

            // Clear the cart after successful order creation
            ClearCartSession();

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

            return orderResponse;
        }

        // Method to get cart items from session
        private List<CartItem> GetCartItemsFromSession()
        {
            const string CartSessionKey = "Cart";

            if (_httpContextAccessor.HttpContext.Session.TryGetValue(CartSessionKey, out byte[] cartData))
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartData);
            }

            return new List<CartItem>();
        }

        // Method to clear cart from session
        private void ClearCartSession()
        {
            const string CartSessionKey = "Cart";
            _httpContextAccessor.HttpContext.Session.Remove(CartSessionKey);
        }
    }
}
