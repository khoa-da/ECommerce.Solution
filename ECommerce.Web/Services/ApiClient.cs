using ECommerce.Web.Utils;

namespace ECommerce.Web.Services
{
    public class ApiClient
    {
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ACCESS_TOKEN_COOKIE = "AccessToken";

        public ApiClient(IHttpContextAccessor httpContextAccessor, string baseUrl = "https://localhost:7062/api/v1/")
        {
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = baseUrl;
        }

        private string? GetAccessTokenFromCookie()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[ACCESS_TOKEN_COOKIE];
        }

        public async Task<T> GetAsync<T>(string endpoint, string? accessToken = null, Dictionary<string, string>? queryParams = null)
        {
            try
            {
                string url = _baseUrl + endpoint;
                // Use provided token or get from cookie
                accessToken ??= GetAccessTokenFromCookie();

                Dictionary<string, string>? headers = null;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    headers = new Dictionary<string, string>
                    {
                        { "Accept-Charset", "utf-8" }
                    };
                }

                var response = await WebUtil.GetAsync(url, headers, accessToken, queryParams);
                var result = WebUtil.HandleResponse<T>(response);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Get Error: {ex.Message}");
                throw;
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object? body, string? accessToken = null, Dictionary<string, string>? queryParams = null)
        {
            try
            {
                string url = _baseUrl + endpoint;
                // Use provided token or get from cookie
                accessToken ??= GetAccessTokenFromCookie();

                Dictionary<string, string>? headers = null;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    headers = new Dictionary<string, string>
                    {
                        { "Accept-Charset", "utf-8" }
                    };
                }

                var response = await WebUtil.PostAsync(url, body, headers, accessToken, queryParams);
                var result = WebUtil.HandleResponse<T>(response);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Post Error: {ex.Message}");
                throw;
            }
        }

        public async Task<T> PutAsync<T>(string endpoint, object? body, string? accessToken = null, Dictionary<string, string>? queryParams = null)
        {
            try
            {
                string url = _baseUrl + endpoint;
                // Use provided token or get from cookie
                accessToken ??= GetAccessTokenFromCookie();

                Dictionary<string, string>? headers = null;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    headers = new Dictionary<string, string>
                    {
                        { "Accept-Charset", "utf-8" }
                    };
                }

                var response = await WebUtil.PutAsync(url, body, headers, accessToken, queryParams);
                var result = WebUtil.HandleResponse<T>(response);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Put Error: {ex.Message}");
                throw;
            }
        }

        public async Task<T> DeleteAsync<T>(string endpoint, string? accessToken = null, Dictionary<string, string>? queryParams = null)
        {
            try
            {
                string url = _baseUrl + endpoint;
                // Use provided token or get from cookie
                accessToken ??= GetAccessTokenFromCookie();

                Dictionary<string, string>? headers = null;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    headers = new Dictionary<string, string>
                    {
                        { "Accept-Charset", "utf-8" }
                    };
                }

                var response = await WebUtil.DeleteAsync(url, headers, accessToken, queryParams);
                var result = WebUtil.HandleResponse<T>(response);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Delete Error: {ex.Message}");
                throw;
            }
        }

        public async Task<T> DeleteWithBodyAsync<T>(string endpoint, object body, string? accessToken = null, Dictionary<string, string>? queryParams = null)
        {
            try
            {
                string url = _baseUrl + endpoint;
                // Use provided token or get from cookie
                accessToken ??= GetAccessTokenFromCookie();

                Dictionary<string, string>? headers = null;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    headers = new Dictionary<string, string>
                    {
                        { "Accept-Charset", "utf-8" }
                    };
                }

                var response = await WebUtil.DeleteAsync(url, body, headers, accessToken, queryParams);
                var result = WebUtil.HandleResponse<T>(response);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Delete with Body Error: {ex.Message}");
                throw;
            }
        }
    }
}
