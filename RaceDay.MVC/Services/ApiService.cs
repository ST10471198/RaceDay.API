using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDay.MVC.Services
{
    public interface IApiService
    {
        Task<(bool IsSuccess, T? Data)> PostAsync<T>(string endpoint, object data);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<(bool IsSuccess, T? Data)> PostAsync<T>(string endpoint, object data)
        {
            // Attach API cookie if it exists
            var apiCookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiCookie");
            if (!string.IsNullOrEmpty(apiCookie))
            {
                _httpClient.DefaultRequestHeaders.Remove("Cookie");
                _httpClient.DefaultRequestHeaders.Add("Cookie", apiCookie);
            }

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);

            // Save API cookie to MVC session
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var sessionCookie = cookies.FirstOrDefault(c => c.StartsWith(".AspNetCore.Session"));
                if (sessionCookie != null)
                {
                    _httpContextAccessor.HttpContext?.Session.SetString("ApiCookie", sessionCookie.Split(';')[0]);
                }
            }

            if (!response.IsSuccessStatusCode) return (false, default);

            var json = await response.Content.ReadAsStringAsync();
            return (true, JsonSerializer.Deserialize<T>(json, _jsonOptions));
        }
    }
}