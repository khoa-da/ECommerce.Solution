using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce.Shared.Models;
using ECommerce.Web.Services.Implementations;
using ECommerce.Web.Services.Interfaces;
using ECommerce.Web.Services;
using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Web.Utils;

namespace ECommerce.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductApiService _productApiService;
        private readonly ApiClient _apiClient;
        private readonly HttpService _httpService;

        public ProductsController(IProductApiService productApiService, ApiClient apiClient, HttpService httpService)
        {
            _productApiService = productApiService;
            _apiClient = apiClient;
            _httpService = httpService;
        }

        // Fix for CS7036: Provide the required 'endpoint' parameter to the GetAsync method.
        public async Task<IActionResult> Index()
        {
            var products = await _httpService.GetAsync<Paginate<ProductResponse>>("products");
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _httpService.GetAsync<ProductDetailResponse>($"products/{id}");
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


    }
}
