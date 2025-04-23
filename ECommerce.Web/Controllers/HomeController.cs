using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Web.Models;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerce.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpService _httpService;

        public HomeController(ILogger<HomeController> logger, HttpService httpService)
        {
            _logger = logger;
            _httpService = httpService;
        }

        public async Task<IActionResult> Index(int page = 1, int size = 10)
        {
            try
            {
                // Gọi API lấy tất cả sản phẩm thay vì theo store
                var response = await _httpService.GetAsync<Paginate<ProductResponse>>($"products?page={page}&size={size}");
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = response.TotalPages;

                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all products");
                ViewBag.ErrorMessage = "An error occurred while loading products. Please try again later.";
                return View(new Paginate<ProductResponse>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}