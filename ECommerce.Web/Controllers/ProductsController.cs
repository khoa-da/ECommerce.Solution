using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Product;
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
