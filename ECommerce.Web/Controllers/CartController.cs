using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpService _httpService;
        private const string CartCookieKey = "Cart";
        private const int CookieExpirationDays = 30; // Cookie sẽ tồn tại trong 30 ngày

        public CartController(HttpService httpService)
        {
            _httpService = httpService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Gọi lại ViewCart để thống nhất logic
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }

            // Lấy thông tin sản phẩm từ service
            var product = await _httpService.GetAsync<ProductDetailResponse>($"products/{productId}");
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction("Index", "Product");
            }

            // Kiểm tra tồn kho
            if (product.Stock < quantity)
            {
                TempData["Error"] = $"There are only {product.Stock} items available in stock";
                return RedirectToAction("Detail", "Product", new { id = productId });
            }

            // Lấy giỏ hàng từ Cookie hoặc tạo mới nếu chưa có
            var cartItems = GetCartFromCookie();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                // Nếu đã có, tăng số lượng
                existingItem.Quantity += quantity;
            }
            else
            {
                // Nếu chưa có, thêm mới vào giỏ hàng
                cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrls?.FirstOrDefault() ?? "",
                    // Có thể thêm các thông tin khác như Size, Color tùy theo yêu cầu
                });
            }

            // Lưu giỏ hàng vào Cookie
            SaveCartToCookie(cartItems);

            TempData["Success"] = "Product added to cart successfully";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        [HttpGet]
        public IActionResult ViewCart()
        {
            var cartItems = GetCartFromCookie();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(Guid productId)
        {
            var cartItems = GetCartFromCookie();
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                SaveCartToCookie(cartItems);
                TempData["Success"] = "Item removed from cart";
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                return RedirectToAction("RemoveFromCart", new { productId });
            }

            var cartItems = GetCartFromCookie();
            var itemToUpdate = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
                SaveCartToCookie(cartItems);
                TempData["Success"] = "Cart updated";
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            // Xóa cookie giỏ hàng
            Response.Cookies.Delete(CartCookieKey);
            TempData["Success"] = "Cart cleared";
            return RedirectToAction("ViewCart");
        }

        // Helper method để lấy giỏ hàng từ Cookie
        private List<CartItem> GetCartFromCookie()
        {
            if (Request.Cookies.TryGetValue(CartCookieKey, out string cartJson))
            {
                try
                {
                    return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                }
                catch
                {
                    // Nếu có lỗi khi deserialize, trả về giỏ hàng mới
                    return new List<CartItem>();
                }
            }

            return new List<CartItem>();
        }

        // Helper method để lưu giỏ hàng vào Cookie
        private void SaveCartToCookie(List<CartItem> cartItems)
        {
            var cartJson = JsonSerializer.Serialize(cartItems);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(CookieExpirationDays),
                HttpOnly = true,
                Secure = Request.IsHttps, // Đảm bảo cookie chỉ được gửi qua HTTPS nếu đang sử dụng HTTPS
                SameSite = SameSiteMode.Lax // Cho phép cookie được gửi trong các yêu cầu chuyển hướng từ bên ngoài
            };

            Response.Cookies.Append(CartCookieKey, cartJson, cookieOptions);
        }
    }
}