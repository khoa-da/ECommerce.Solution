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
        private const string CartSessionKey = "Cart";

        public CartController(HttpService httpService)
        {
            _httpService = httpService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            // Gọi lại ViewCart để thống nhất logic (nếu bạn muốn giữ ViewCart làm chính)
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

            // Lấy giỏ hàng từ Session hoặc tạo mới nếu chưa có
            var cartItems = GetCartFromSession();

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

            // Lưu giỏ hàng vào Session
            SaveCartToSession(cartItems);

            TempData["Success"] = "Product added to cart successfully";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        [HttpGet]
        public IActionResult ViewCart()
        {
            var cartItems = GetCartFromSession();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(Guid productId)
        {
            var cartItems = GetCartFromSession();
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                SaveCartToSession(cartItems);
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

            var cartItems = GetCartFromSession();
            var itemToUpdate = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
                SaveCartToSession(cartItems);
                TempData["Success"] = "Cart updated";
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            TempData["Success"] = "Cart cleared";
            return RedirectToAction("ViewCart");
        }

        // Helper method để lấy giỏ hàng từ Session
        private List<CartItem> GetCartFromSession()
        {
            if (HttpContext.Session.TryGetValue(CartSessionKey, out byte[] cartData))
            {
                return JsonSerializer.Deserialize<List<CartItem>>(cartData);
            }

            return new List<CartItem>();
        }

        // Helper method để lưu giỏ hàng vào Session
        private void SaveCartToSession(List<CartItem> cartItems)
        {
            var cartData = JsonSerializer.SerializeToUtf8Bytes(cartItems);
            HttpContext.Session.Set(CartSessionKey, cartData);
        }
    }


}