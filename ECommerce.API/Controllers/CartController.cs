using ECommerce.Core.Services.Interfaces;
using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : BaseController<CartController>
    {
        private readonly ICartService _cartService;
        private readonly string _guestCartCookieName = "guest_cart_id";
        private readonly TimeSpan _guestCartCookieExpiry = TimeSpan.FromDays(7);
        public CartController(ILogger<CartController> logger, ICartService cartService) : base(logger)
        {
            _cartService = cartService;
        }

        [HttpGet(ApiEndPointConstant.Cart.CartsEndpoint)]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCart()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (User.Identity.IsAuthenticated)
            {
                // Người dùng đã đăng nhập
                var userId = GetUserId();
                var cart = await _cartService.GetUserCartAsync(userId);

                if (cart == null)
                {
                    cart = await _cartService.CreateCartAsync(userId);
                }

                // Kiểm tra xem có giỏ hàng của khách (cookie) không
                if (Request.Cookies.TryGetValue(_guestCartCookieName, out string guestCartId))
                {
                    // Hợp nhất giỏ hàng khách với giỏ hàng người dùng
                    var guestCart = await _cartService.GetGuestCartAsync(guestCartId);
                    if (guestCart != null && guestCart.Items.Count > 0)
                    {
                        cart = await _cartService.MergeCartsAsync(guestCart.Id, userId);

                        // Xóa cookie giỏ hàng khách
                        Response.Cookies.Delete(_guestCartCookieName);
                    }
                }

                return Ok(cart);
            }
            else
            {
                // Khách (guest)
                if (!Request.Cookies.TryGetValue(_guestCartCookieName, out string guestId))
                {
                    // Tạo ID khách mới
                    guestId = Guid.NewGuid().ToString();
                    SetGuestCartCookie(guestId);
                }

                var cart = await _cartService.GetGuestCartAsync(guestId);
                if (cart == null)
                {
                    cart = await _cartService.CreateGuestCartAsync(guestId);
                }

                return Ok(cart);
            }
        }

        [HttpPost(ApiEndPointConstant.Cart.CartsEndpoint)]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            Cart cart;

            if (User.Identity.IsAuthenticated)
            {
                // Người dùng đã đăng nhập
                var userId = GetUserId();
                cart = await _cartService.GetUserCartAsync(userId);

                if (cart == null)
                {
                    cart = await _cartService.CreateCartAsync(userId);
                }
            }
            else
            {
                // Khách (guest)
                string guestId;
                if (!Request.Cookies.TryGetValue(_guestCartCookieName, out guestId))
                {
                    guestId = Guid.NewGuid().ToString();
                    SetGuestCartCookie(guestId);
                }

                cart = await _cartService.GetGuestCartAsync(guestId);
                if (cart == null)
                {
                    cart = await _cartService.CreateGuestCartAsync(guestId);
                }
            }

            var updatedCart = await _cartService.AddToCartAsync(cart.Id, item);
            return Ok(updatedCart);
        }
        [HttpPut(ApiEndPointConstant.Cart.CartsEndpoint)]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItem item)
        {
            Cart cart;

            if (User.Identity.IsAuthenticated)
            {
                var userId = GetUserId();
                cart = await _cartService.GetUserCartAsync(userId);
            }
            else
            {
                if (!Request.Cookies.TryGetValue(_guestCartCookieName, out string guestId))
                {
                    return NotFound("Cart not found");
                }

                cart = await _cartService.GetGuestCartAsync(guestId);
            }

            if (cart == null)
                return NotFound("Cart not found");

            var updatedCart = await _cartService.UpdateCartItemAsync(cart.Id, item);
            return Ok(updatedCart);
        }

        [HttpDelete(ApiEndPointConstant.Cart.CartItemEndpointByProductId)]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveFromCart(Guid productId)
        {
            Cart cart;

            if (User.Identity.IsAuthenticated)
            {
                var userId = GetUserId();
                cart = await _cartService.GetUserCartAsync(userId);
            }
            else
            {
                if (!Request.Cookies.TryGetValue(_guestCartCookieName, out string guestId))
                {
                    return NotFound("Cart not found");
                }

                cart = await _cartService.GetGuestCartAsync(guestId);
            }

            if (cart == null)
                return NotFound("Cart not found");

            var updatedCart = await _cartService.RemoveFromCartAsync(cart.Id, productId);
            return Ok(updatedCart);
        }

        [HttpDelete(ApiEndPointConstant.Cart.CartsEndpoint)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCart()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = GetUserId();
                var cart = await _cartService.GetUserCartAsync(userId);

                if (cart == null)
                    return NotFound("Cart not found");

                await _cartService.DeleteCartAsync(cart.Id);
            }
            else
            {
                if (!Request.Cookies.TryGetValue(_guestCartCookieName, out string guestId))
                {
                    return NotFound("Cart not found");
                }

                await _cartService.DeleteGuestCartAsync(guestId);
                Response.Cookies.Delete(_guestCartCookieName);
            }

            return Ok(true); // 200 with body: true
        }


        [HttpPost(ApiEndPointConstant.Cart.MergeCartEndpoint)]
        public async Task<IActionResult> MergeCarts()
        {
            if (!Request.Cookies.TryGetValue(_guestCartCookieName, out string guestId))
            {
                return BadRequest("No guest cart found");
            }

            var guestCart = await _cartService.GetGuestCartAsync(guestId);
            if (guestCart == null)
            {
                return BadRequest("Guest cart not found");
            }

            var userId = GetUserId();
            var mergedCart = await _cartService.MergeCartsAsync(guestCart.Id, userId);

            // Xóa cookie giỏ hàng khách
            Response.Cookies.Delete(_guestCartCookieName);

            return Ok(mergedCart);
        }
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim);
        }
        private void SetGuestCartCookie(string guestId)
        {
            Response.Cookies.Append(_guestCartCookieName, guestId, new CookieOptions
            {
                Expires = DateTime.UtcNow.Add(_guestCartCookieExpiry),
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax
            });
        }
    }
}
