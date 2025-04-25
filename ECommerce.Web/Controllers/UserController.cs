using ECommerce.Shared.Payload.Response.User;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpService _httpService;
        public UserController(HttpService httpService)
        {
            _httpService = httpService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = GetUserIdFromJwt();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                var userResponse = await _httpService.GetAsync<UserResponse>($"users/{userId}");
                return View(userResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user profile: {ex.Message}");
                TempData["Error"] = "Unable to load user profile. Please try again later.";
                return RedirectToAction("Index", "Home");
            }
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
