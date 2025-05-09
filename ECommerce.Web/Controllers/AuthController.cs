using ECommerce.Shared.BusinessModels;
using ECommerce.Shared.Payload.Request.Auth;
using ECommerce.Shared.Payload.Response.Auth;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpService _httpService;
        private readonly ILogger<AuthController> _logger;

        // 👉 Khớp với scheme đã config trong Program.cs
        private const string AuthScheme = "MyCookieAuth";

        public AuthController(ILogger<AuthController> logger, HttpService httpService)
        {
            _logger = logger;
            _httpService = httpService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            // If user is already logged in, redirect to home page
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {

                // Prepare the request body
                var requestBody = new
                {
                    username = model.Email,  // Using email as username
                    email = model.Email,
                    passwordHash = model.Password,  // Password will be hashed by the API
                    firstName = model.FullName,
                    lastName = model.FullName,
                    phoneNumber = model.PhoneNumber,
                    emailConfirmed = false  // Default to false, will be confirmed later
                };

                var response = await _httpService.PostAsync<object>("users", requestBody);

                // Successful registration
                TempData["SuccessMessage"] = "Your account has been created successfully. Please log in.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error during registration: {ex.Message}");

                // Add specific error message for user
                ViewBag.ErrorMessage = "Registration failed. Please try again later.";

                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginRequest = new LoginRequest
                {
                    UsernameOrEmail = model.UsernameOrEmail,
                    Password = model.Password
                };

                var response = await _httpService.PostAsync<LoginResponse>("auth/login", loginRequest);

                if (response != null && !string.IsNullOrEmpty(response.AccessToken))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, model.UsernameOrEmail),
                        new Claim("Token", response.AccessToken),
                        new Claim(ClaimTypes.Name, response.FirstName ?? model.UsernameOrEmail),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, AuthScheme);

                    await HttpContext.SignInAsync(
                        AuthScheme, // ✅ Dùng đúng tên scheme đã config 
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt");
                ViewBag.ErrorMessage = "An error occurred during login. Please try again.";
                return View(model);
            }
        }
    }
}
