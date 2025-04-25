using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Request.Order;
using ECommerce.Shared.Payload.Response.Order;
using ECommerce.Shared.Payload.Response.User;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpService _httpService;
        private const string CartCookieKey = "Cart";
        protected IHttpContextAccessor _httpContextAccessor;

        public OrderController(HttpService httpService, IHttpContextAccessor httpContextAccessor)
        {
            _httpService = httpService;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            // Get cart items from cookies
            var cartItems = GetCartItemsFromCookies();

            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["Error"] = "Your cart is empty. Add some products before checking out.";
                return RedirectToAction("ViewCart", "Cart");
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems
            };
            var userId = GetUserIdFromJwt();
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    var user = await _httpService.GetAsync<UserResponse>($"users/{Guid.Parse(userId)}");
                    if (user != null)
                    {
                        checkoutViewModel.FirstName = user.FirstName;
                        checkoutViewModel.LastName = user.LastName;
                        checkoutViewModel.Email = user.Email;
                        checkoutViewModel.PhoneNumber = user.PhoneNumber;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue - user can still manually enter info
                    Console.WriteLine($"Error fetching user data: {ex.Message}");
                }
            }
            else
            {
                TempData["Error"] = "You need to be logged in to checkout.";
                return RedirectToAction("Login", "Auth");
            }

            return View(checkoutViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CancelOrderForm(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Order ID is required.";
                return RedirectToAction("OrderHistory");
            }
            var order = await _httpService.GetAsync<OrderResponse>($"orders/{id}");
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("OrderHistory");
            }
            if (order.OrderStatus != "Processing" && order.OrderStatus != "Pending")
            {
                TempData["Error"] = "This order cannot be cancelled anymore.";
                return RedirectToAction("Details", new { id });
            }
            var cancelOrderViewModel = new CancelOrderFormViewModel
            {
                OrderId = Guid.Parse(id),

            };
            return View(cancelOrderViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CancelOrder(CancelOrderFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data.";
                return View(model);
            }
            try
            {
                var cancelOrderRequest = new CancelOrderRequest
                {
                    OrderId = model.OrderId,
                    Reason = model.Reason + " - " + model.Comments
                };

                var order = await _httpService.GetAsync<OrderResponse>($"orders/{model.OrderId}");
                if(order == null || order.UserId != Guid.Parse(GetUserIdFromJwt()))
                {
                    TempData["Error"] = "You are not authorized to cancel this order.";
                    return RedirectToAction("OrderHistory");
                }
                if (order.OrderStatus != "Processing" && order.OrderStatus != "Pending")
                {
                    TempData["Error"] = "This order cannot be cancelled anymore.";
                    return RedirectToAction("Details", new { id = model.OrderId });
                }
                var response = await _httpService.PutAsync<CancelOrderResponse>($"orders/cancel", cancelOrderRequest);

                if (response.CancelAt != null)
                {
                    TempData["Success"] = "Your order has been cancelled successfully!";
                    return RedirectToAction("OrderHistory");
                }
                else
                {
                    TempData["Error"] = "Failed to cancel order. Please try again.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder(CheckoutViewModel model)
        {


            try
            {
                // Get cart items from cookies
                var cartItems = GetCartItemsFromCookies();
                var userId = GetUserIdFromJwt();

                //If userId is null, redirect to login page
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "You need to be logged in to place an order.";
                    return RedirectToAction("Login", "Auth");
                }

                var userGuid = Guid.Parse(userId);

                //Get user from userId to ensure we have latest data
                var user = await _httpService.GetAsync<UserResponse>($"users/{userGuid}");
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Login", "Auth");
                }

                if (cartItems == null || cartItems.Count == 0)
                {
                    TempData["Error"] = "Your cart is empty. Add some products before placing an order.";
                    return RedirectToAction("ViewCart", "Cart");
                }

                // Use user data from database instead of form submission for these fields
                // Only use Address from the form
                // Create order request
                var fullAddress = $"{model.Address}, {model.City}, {model.State}, {model.ZipCode}, {model.Country}".Trim().TrimEnd(',');

                var orderRequest = new OrderRequest
                {
                    UserId = userGuid,
                    ShippingAddress = fullAddress,
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethod = model.ShippingMethod,
                    CartItems = cartItems,
                    Notes = model.Notes
                };


                // Call API to create order
                var orderResponse = await _httpService.PostAsync<OrderResponse>("orders", orderRequest);

                if (orderResponse != null)
                {
                    // Order created successfully
                    // Clear the cart cookie
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete(CartCookieKey);

                    TempData["Success"] = "Your order has been placed successfully!";
                    return RedirectToAction("OrderConfirmation", new { id = orderResponse.Id });
                }
                else
                {
                    TempData["Error"] = "Failed to create order. Please try again.";
                    return View("Checkout", model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View("Checkout", model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> OrderHistory(string? search, string? orderBy, int page = 1, int size = 10)
        {
            try
            {
                var userId = GetUserIdFromJwt();
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "You need to be logged in to view your order history.";
                    return RedirectToAction("Login", "Auth");
                }
                var userGuid = Guid.Parse(userId);
                var queryParams = new List<string>
        {
                    $"page={page}",
                    $"size={size}"
        };
                if (!string.IsNullOrEmpty(orderBy))
                {
                    queryParams.Add($"orderBy={orderBy}");
                }

                if (!string.IsNullOrEmpty(search))
                {
                    queryParams.Add($"search={search}");
                }
                var apiUrl = $"users/{userId}/order?{string.Join("&", queryParams)}";
                var apiResponse = await _httpService.GetAsync<Paginate<OrderResponse>>(apiUrl);
                if (apiResponse == null)
                {
                    TempData["Error"] = "No orders found.";
                    return View(new Paginate<OrderResponse>());
                }
                // set pagination properties in ViewBag
                ViewBag.CurrentPage = apiResponse.Page;
                ViewBag.TotalPages = apiResponse.TotalPages;
                ViewBag.TotalItems = apiResponse.Total;
                ViewBag.PageSize = apiResponse.Size;
                return View(apiResponse);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(new Paginate<OrderResponse>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(Guid id)
        {
            try
            {
                // Get order details
                var order = await _httpService.GetAsync<OrderResponse>($"orders/{id}");

                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index", "Home");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            try
            {
                // Get order details
                var order = await _httpService.GetAsync<OrderResponse>($"orders/{id}");
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index", "Home");
                }
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // Helper method to get cart items from cookies
        private List<CartItem> GetCartItemsFromCookies()
        {
            const string CartCookieKey = "Cart";

            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(CartCookieKey, out string cartJson))
            {
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                }
                catch
                {
                    // If there's an error when deserializing, return a new cart
                    return new List<CartItem>();
                }
            }

            return new List<CartItem>();
        }
        private string GetUserIdFromJwt()
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
                return null;

            // Get access token from claims
            var tokenClaim = User.Claims.FirstOrDefault(c => c.Type == "Token");
            if (tokenClaim == null || string.IsNullOrEmpty(tokenClaim.Value))
                return null;

            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenClaim.Value);

                // Get the user ID claim from the token
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return userIdClaim?.Value;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error extracting user ID from JWT: {ex.Message}");
                return null;
            }
        }


    }
}
