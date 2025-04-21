using System.Diagnostics;
using ECommerce.Shared.Payload.Response.Product;
using System.Net.Http;
using System.Text.Json;
using ECommerce.Web.Models;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Store;

namespace ECommerce.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpService _httpService;
        private readonly string _defaultStoreId = "98ECB2CF-F3D0-47D5-B3B9-3F08B6921FC1";

        public HomeController(ILogger<HomeController> logger, HttpService httpService)
        {
            _logger = logger;
            _httpService = httpService;
        }

        public async Task<IActionResult> Index(string storeId = null, string customStoreId = null, int page = 1, int size = 10)
        {
            if (!string.IsNullOrEmpty(customStoreId))
            {
                storeId = customStoreId;
            }
            storeId = string.IsNullOrWhiteSpace(storeId) ? _defaultStoreId : storeId;

            try
            {
                var stores = await _httpService.GetAsync<Paginate<StoreResponse>>("stores");
                ViewBag.Stores = stores;

                var response = await _httpService.GetAsync<Paginate<ProductResponse>>($"stores/{storeId}/products?page={page}&size={size}");
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = response.TotalPages;
                ViewBag.StoreId = storeId;
                //ViewBag.StoreName = 
                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching products for store ID: {storeId}");
                ViewBag.ErrorMessage = "An error occurred while loading products. Please try again later.";
                ViewBag.StoreId = storeId;
                return View(new ProductResponse());
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
