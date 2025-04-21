using ECommerce.Shared.Paginate;
using ECommerce.Shared.Payload.Response.Product;
using ECommerce.Web.Services.Interfaces;

namespace ECommerce.Web.Services.Implementations
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;
        public ProductApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ECommerceAPI");
        }
        public Task<IPaginate<ProductResponse>> GetAll(string? search, string? orderBy, int page, int size)
        {
            var products = _httpClient.GetFromJsonAsync<IPaginate<ProductResponse>>($"api/products?search={search}&orderBy={orderBy}&page={page}&size={size}");
            return products;
        }
    }
}
