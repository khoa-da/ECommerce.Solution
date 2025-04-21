using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ECommerce.Web.Utils
{
    public class HttpService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;
        private const string ACCESS_TOKEN_COOKIE = "AccessToken";

        public HttpService(IHttpContextAccessor httpContextAccessor, string baseUrl = "https://localhost:7062/api/v1/")
        {
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = baseUrl;
        }

        private string? GetAccessTokenFromCookie()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[ACCESS_TOKEN_COOKIE];
        }

        // GET request
        public async Task<T> GetAsync<T>(string endpoint, string? token = null, Dictionary<string, string>? queryParams = null)
        {
            token ??= GetAccessTokenFromCookie();
            string url = _baseUrl + endpoint;

            // Add query parameters if needed
            if (queryParams != null && queryParams.Count > 0)
            {
                var query = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                url = $"{url}?{query}";
            }

            using var httpClient = new HttpClient();

            // Add authorization if token provided
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync(url);
            return await DeserializeResponse<T>(response);
            }

        // POST request
        public async Task<T> PostAsync<T>(string endpoint, object data, string? token = null, Dictionary<string, string>? queryParams = null)
        {
            token ??= GetAccessTokenFromCookie();
            string url = _baseUrl + endpoint;

            // Add query parameters if needed
            if (queryParams != null && queryParams.Count > 0)
            {
                var query = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                url = $"{url}?{query}";
            }

            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();

            // Add authorization if token provided
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await httpClient.PostAsync(url, content);
            return await DeserializeResponse<T>(response);
        }

        // PUT request
        public async Task<T> PutAsync<T>(string endpoint, object data, string? token = null, Dictionary<string, string>? queryParams = null)
        {
            token ??= GetAccessTokenFromCookie();
            string url = _baseUrl + endpoint;

            // Add query parameters if needed
            if (queryParams != null && queryParams.Count > 0)
            {
                var query = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                url = $"{url}?{query}";
            }

            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();

            // Add authorization if token provided
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await httpClient.PutAsync(url, content);
            return await DeserializeResponse<T>(response);
        }

        // DELETE request
        public async Task<T> DeleteAsync<T>(string endpoint, string? token = null, Dictionary<string, string>? queryParams = null)
        {
            token ??= GetAccessTokenFromCookie();
            string url = _baseUrl + endpoint;

            // Add query parameters if needed
            if (queryParams != null && queryParams.Count > 0)
            {
                var query = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                url = $"{url}?{query}";
            }

            using var httpClient = new HttpClient();

            // Add authorization if token provided
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await httpClient.DeleteAsync(url);
            return await DeserializeResponse<T>(response);
        }

        // DELETE with body
        public async Task<T> DeleteWithBodyAsync<T>(string endpoint, object data, string? token = null, Dictionary<string, string>? queryParams = null)
        {
            token ??= GetAccessTokenFromCookie();
            string url = _baseUrl + endpoint;

            // Add query parameters if needed
            if (queryParams != null && queryParams.Count > 0)
            {
                var query = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                url = $"{url}?{query}";
            }

            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();

            // Add authorization if token provided
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = content
            };

            var response = await httpClient.SendAsync(request);
            return await DeserializeResponse<T>(response);
        }

        // Helper method to deserialize response
        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            try
            {
                var responseString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(responseString))
                {
                    throw new InvalidOperationException("Response content is empty.");
                }

                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing response: {ex.Message}");
                throw;
            }
        }
    }
}
