// ShopController.cs (Gọi API từ HttpService)

using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Shared.Payload.Response.Category;
using ECommerce.Web.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpService _httpService;

        public ProductsController(HttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<IActionResult> Index(string? search, string? sortBy = "created_desc", int page = 1, int pageSize = 12)
        {
            await SetCommonViewBags(sortBy, pageSize, search);
            string endpoint = $"products?page={page}&size={pageSize}&orderBy={sortBy}";
            if (!string.IsNullOrWhiteSpace(search))
                endpoint += $"&search={search}";

            var result = await _httpService.GetAsync<Paginate<ProductResponse>>(endpoint);

           
            return View(result);
        }
        public async Task<IActionResult> Search(string query, string? sortBy = "created_desc", int page = 1, int pageSize = 12)
        {
            // Chuyển hướng đến action Index với tham số search
            return RedirectToAction("Index", new { search = query, sortBy, page, pageSize });
        }


        public async Task<IActionResult> Category(Guid categoryId, string? search, string? sortBy = "created_desc", int page = 1, int pageSize = 12)
        {
            await SetCommonViewBags(sortBy, pageSize, search);
            string endpoint = $"categories/{categoryId}/products?page={page}&size={pageSize}&orderBy={sortBy}";
            if (!string.IsNullOrWhiteSpace(search))
                endpoint += $"&search={search}";

            var result = await _httpService.GetAsync<Paginate<ProductResponse>>(endpoint);

            
            ViewBag.ActiveFilters = new Dictionary<string, string> { { "CategoryId", categoryId.ToString() } };
            return View("Index", result);
        }

        public async Task<IActionResult> ParentCategory(Guid parentCategoryId, string? search, string? sortBy = "created_desc", int page = 1, int pageSize = 12)
        {
            await SetCommonViewBags(sortBy, pageSize, search);
            string endpoint = $"categories/parent/{parentCategoryId}/products?page={page}&size={pageSize}&orderBy={sortBy}";
            if (!string.IsNullOrWhiteSpace(search))
                endpoint += $"&search={search}";

            var result = await _httpService.GetAsync<Paginate<ProductResponse>>(endpoint);

            
            ViewBag.ActiveFilters = new Dictionary<string, string> { { "ParentCategoryId", parentCategoryId.ToString() } };
            return View("Index", result);
        }

        public async Task<IActionResult> Brand(string brand, int page = 1, int pageSize = 12)
        {
            await SetCommonViewBags("created_desc", pageSize, null);
            string endpoint = $"products/brand/{brand}?page={page}&size={pageSize}";
            var result = await _httpService.GetAsync<Paginate<ProductResponse>>(endpoint);

            
            ViewBag.ActiveFilters = new Dictionary<string, string> { { "Brand", brand } };
            return View("Index", result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _httpService.GetAsync<ProductDetailResponse>($"products/{id}");
            return View(result);
        }

        private async Task SetCommonViewBags(string sortBy, int pageSize, string? search)
        {
            ViewBag.SortBy = sortBy;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchQuery = search;

            var parentCategoryResponse = await _httpService.GetAsync<Paginate<CategoryResponse>>("categories/parents?page=1&size=20");
            var childCategoryResponse = await _httpService.GetAsync<Paginate<CategoryResponse>>("categories/children?page=1&size=100");

            //ViewBag.ParentCategories = parentCategoryResponse?.Items ?? new List<CategoryResponse>();
            ViewData["ParentCategories"] = parentCategoryResponse?.Items ?? new List<CategoryResponse>();

            var childGroups = childCategoryResponse?.Items?
                .Where(c => c.ParentId.HasValue)
                .GroupBy(c => c.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList()) ?? new();

            //ViewBag.ChildCategories = childGroups;
            ViewData["ChildCategories"] = childGroups;
        }
    }
}
