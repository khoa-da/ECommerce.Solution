using AutoMapper;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Core.Services.Implementations
{
    public class CartService : BaseService<CartService>, ICartService
    {
        private readonly IConnectionMultiplexer _redis;
        //private readonly StackExchange.Redis.IDatabase _database;
        private readonly string _cartPrefix = "cart:";
        private readonly string _userCartPrefix = "user:cart:";
        private readonly string _guestCartPrefix = "guest:cart:";
        private readonly TimeSpan _cartExpiry = TimeSpan.FromDays(30);
        private readonly TimeSpan _guestCartExpiry = TimeSpan.FromDays(7); // Giỏ hàng của khách hết hạn sớm hơn

        public CartService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<CartService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer redis) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _redis = redis;

        }

        public async Task<Cart> AddToCartAsync(string cartId, CartItem item)
        {
            var cart = await GetCartAsync(cartId);
            if (cart == null)
            {
                throw new Exception("Cart not found");
            }
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }
            cart.UpdatedAt = DateTime.UtcNow.AddHours(7);

            await SaveCartAsync(cart);
            return cart;
        }

        private async Task SaveCartAsync(Cart cart)
        {
            var key = _cartPrefix + cart.Id;
            var cartJson = JsonSerializer.Serialize(cart);
            var expiry = cart.UserId == Guid.Empty ? _guestCartExpiry : _cartExpiry;
            await _redis.GetDatabase().StringSetAsync(key, cartJson, expiry);
        }

        public async Task<Cart> CreateCartAsync(Guid? userId = null)
        {
            string cartId;

            if (userId.HasValue && userId.Value != Guid.Empty)
            {
                // Người dùng đã đăng nhập
                var existingCart = await GetUserCartAsync(userId.Value);
                if (existingCart != null)
                {
                    return existingCart;
                }

                cartId = Guid.NewGuid().ToString();
                var cart = new Cart
                {
                    Id = cartId,
                    UserId = userId.Value,
                    Items = new List<CartItem>(),
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                var key = _cartPrefix + cartId;
                var cartJson = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
                await _redis.GetDatabase().StringSetAsync(key, cartJson, _cartExpiry);

                // Liên kết giỏ hàng với người dùng 
                var userCartKey = _userCartPrefix + userId.Value.ToString();
                await _redis.GetDatabase().StringSetAsync(userCartKey, cartId, _cartExpiry);

                return cart;
            }
            else
            {
                // Khách (guest)
                cartId = Guid.NewGuid().ToString();
                var cart = new Cart
                {
                    Id = cartId,
                    UserId = Guid.Empty, // Đánh dấu là guest cart
                    Items = new List<CartItem>(),
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                var key = _cartPrefix + cartId;
                var cartJson = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
                await _redis.GetDatabase().StringSetAsync(key, cartJson, _guestCartExpiry);

                return cart;
            }
        }

        // Tạo giỏ hàng cho khách với ID (thường là cookie ID)
        public async Task<Cart> CreateGuestCartAsync(string guestId)
        {
            if (string.IsNullOrEmpty(guestId))
            {
                throw new ArgumentException("Guest ID is required");
            }

            var existingCart = await GetGuestCartAsync(guestId);
            if (existingCart != null)
            {
                return existingCart;
            }

            var cartId = Guid.NewGuid().ToString();
            var cart = new Cart
            {
                Id = cartId,
                UserId = Guid.Empty, // Đánh dấu là guest cart
                Items = new List<CartItem>(),
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            };

            var key = _cartPrefix + cartId;
            var cartJson = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
            await _redis.GetDatabase().StringSetAsync(key, cartJson, _guestCartExpiry);

            // Liên kết giỏ hàng với guest ID
            var guestCartKey = _guestCartPrefix + guestId;
            await _redis.GetDatabase().StringSetAsync(guestCartKey, cartId, _guestCartExpiry);

            return cart;
        }

        public async Task<Cart> GetGuestCartAsync(string guestId)
        {
            if (string.IsNullOrEmpty(guestId))
            {
                return null;
            }

            var guestCartKey = _guestCartPrefix + guestId;
            var cartId = await _redis.GetDatabase().StringGetAsync(guestCartKey);

            if (cartId.IsNullOrEmpty)
            {
                return null;
            }

            return await GetCartAsync(cartId.ToString());
        }

        public async Task<bool> DeleteCartAsync(string cartId)
        {
            var cart = await GetCartAsync(cartId);
            if (cart == null)
                return false;

            // Delete cart
            var key = _cartPrefix + cartId;
            var success = await _redis.GetDatabase().KeyDeleteAsync(key);

            // Remove user-cart association if it's a user cart
            if (cart.UserId != Guid.Empty)
            {
                var userCartKey = _userCartPrefix + cart.UserId.ToString();
                await _redis.GetDatabase().KeyDeleteAsync(userCartKey);
            }

            return success;
        }

        public async Task<bool> DeleteGuestCartAsync(string guestId)
        {
            var guestCartKey = _guestCartPrefix + guestId;
            var cartId = await _redis.GetDatabase().StringGetAsync(guestCartKey);

            if (cartId.IsNullOrEmpty)
            {
                return false;
            }

            // Delete the cart
            var key = _cartPrefix + cartId.ToString();
            var success = await _redis.GetDatabase().KeyDeleteAsync(key);

            // Delete the guest-cart association
            await _redis.GetDatabase().KeyDeleteAsync(guestCartKey);

            return success;
        }

        public async Task<Cart> GetCartAsync(string cartId)
        {
            var key = _cartPrefix + cartId;
            var cartData = await _redis.GetDatabase().StringGetAsync(key);
            if (cartData.IsNullOrEmpty)
            {
                return null;
            }

            try
            {
                var cart = Newtonsoft.Json.JsonConvert.DeserializeObject<Cart>(cartData);
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing cart data");
                return null;
            }
        }

        public async Task<Cart> GetUserCartAsync(Guid userId)
        {
            var userCartKey = _userCartPrefix + userId.ToString();
            var cartId = _redis.GetDatabase().StringGet(userCartKey);
            if (cartId.IsNullOrEmpty)
            {
                return null;
            }
            var cart = await GetCartAsync(cartId.ToString());
            return cart;
        }

        public async Task<Cart> RemoveFromCartAsync(string cartId, Guid productId)
        {
            var cart = await GetCartAsync(cartId);
            if (cart == null)
                return null;

            var existingItem = cart.Items.Find(i => i.ProductId == productId);
            if (existingItem != null)
            {
                cart.Items.Remove(existingItem);
                cart.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await SaveCartAsync(cart);
            }

            return cart;
        }

        public async Task<Cart> UpdateCartItemAsync(string cartId, CartItem item)
        {
            var cart = await GetCartAsync(cartId);
            if (cart == null) return null;

            var existtingIten = cart.Items.Find(i => i.ProductId == item.ProductId);
            if (existtingIten == null)
            {
                return cart;
            }
            if (item.Quantity <= 0)
            {
                cart.Items.Remove(existtingIten);
            }
            else
            {
                existtingIten.Quantity = item.Quantity;
                existtingIten.Price = item.Price;
                if (!string.IsNullOrEmpty(item.ProductName))
                {
                    existtingIten.ProductName = item.ProductName;
                }
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    existtingIten.ImageUrl = item.ImageUrl;
                }
            }
            cart.UpdatedAt = DateTime.UtcNow.AddHours(7);
            await SaveCartAsync(cart);
            return cart;
        }

        // Hợp nhất giỏ hàng của khách và người dùng khi đăng nhập
        public async Task<Cart> MergeCartsAsync(string guestCartId, Guid userId)
        {
            var guestCart = await GetCartAsync(guestCartId);
            if (guestCart == null || !guestCart.Items.Any())
            {
                // Nếu giỏ hàng khách trống, chỉ cần lấy giỏ hàng người dùng
                return await GetUserCartAsync(userId) ?? await CreateCartAsync(userId);
            }

            var userCart = await GetUserCartAsync(userId);
            if (userCart == null)
            {
                // Nếu người dùng chưa có giỏ hàng, tạo mới
                userCart = await CreateCartAsync(userId);
            }

            // Hợp nhất các sản phẩm
            foreach (var item in guestCart.Items)
            {
                var existingItem = userCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    // Nếu sản phẩm đã tồn tại, tăng số lượng
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    // Nếu sản phẩm chưa tồn tại, thêm vào
                    userCart.Items.Add(item);
                }
            }

            userCart.UpdatedAt = DateTime.UtcNow.AddHours(7);
            await SaveCartAsync(userCart);

            // Xóa giỏ hàng của khách
            await DeleteCartAsync(guestCartId);

            return userCart;
        }
    }
}