using ECommerce.Shared.Models;
using ECommerce.Shared.Payload.Request.Rating;
using ECommerce.Shared.Payload.Response.Rating;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Web.Controllers
{
    public class RatingController : Controller
    {
        private readonly HttpService _httpService;

        public RatingController(HttpService httpService)
        {
            _httpService = httpService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(RatingRequest request)
        {
            // Kiểm tra tính hợp lệ của request


            // Kiểm tra điểm đánh giá nằm trong khoảng từ 1 đến 5
            if (request.Score < 1 || request.Score > 5)
            {
                TempData["ErrorMessage"] = "Please select a rating between 1 and 5 stars.";
                return RedirectToAction("OrderHistory", "Order");
            }

            try
            {
                var userId = GetUserIdFromJwt();
                request.UserId = Guid.Parse(userId);
                // Gọi API để gửi đánh giá
                var response = await _httpService.PostAsync<RatingResponse>("ratings", request);

                if (response != null && response.Id != Guid.Empty)
                {
                    TempData["SuccessMessage"] = "Thank you for your review!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error submitting review. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("OrderHistory", "Order");
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
