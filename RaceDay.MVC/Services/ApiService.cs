using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDay.MVC.Services
{
    public interface IApiService
    {
        Task<(bool IsSuccess, T? Data)> PostAsync<T>(string endpoint, object data);
        Task<T?> GetAsync<T>(string endpoint);
        Task<(bool IsSuccess, T? Data)> PostFileAsync<T>(string endpoint, MultipartFormDataContent content);
        Task<(bool IsSuccess, T? Data)> PutFileAsync<T>(string endpoint, MultipartFormDataContent content);
        Task<bool> DeleteAsync(string endpoint);
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

        private void AttachSessionCookie()
        {
            var apiCookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiCookie");
            if (!string.IsNullOrEmpty(apiCookie))
            {
                _httpClient.DefaultRequestHeaders.Remove("Cookie");
                _httpClient.DefaultRequestHeaders.Add("Cookie", apiCookie);
            }
        }

        private void SaveSessionCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var sessionCookie = cookies.FirstOrDefault(c => c.StartsWith(".AspNetCore.Session"));
                if (sessionCookie != null)
                {
                    _httpContextAccessor.HttpContext?.Session.SetString("ApiCookie", sessionCookie.Split(';')[0]);
                }
            }
        }

        public async Task<(bool IsSuccess, T? Data)> PostAsync<T>(string endpoint, object data)
        {
            AttachSessionCookie();
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            SaveSessionCookie(response);

            if (!response.IsSuccessStatusCode) return (false, default);

            var json = await response.Content.ReadAsStringAsync();
            return (true, JsonSerializer.Deserialize<T>(json, _jsonOptions));
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            AttachSessionCookie();
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode) return default;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task<(bool IsSuccess, T? Data)> PostFileAsync<T>(string endpoint, MultipartFormDataContent content)
        {
            AttachSessionCookie();
            var response = await _httpClient.PostAsync(endpoint, content);
            SaveSessionCookie(response);

            if (!response.IsSuccessStatusCode) return (false, default);

            var json = await response.Content.ReadAsStringAsync();
            return (true, JsonSerializer.Deserialize<T>(json, _jsonOptions));
        }

        public async Task<(bool IsSuccess, T? Data)> PutFileAsync<T>(string endpoint, MultipartFormDataContent content)
        {
            AttachSessionCookie();
            var response = await _httpClient.PutAsync(endpoint, content);
            SaveSessionCookie(response);

            if (!response.IsSuccessStatusCode) return (false, default);

            var json = await response.Content.ReadAsStringAsync();
            return (true, JsonSerializer.Deserialize<T>(json, _jsonOptions));
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            AttachSessionCookie();
            var response = await _httpClient.DeleteAsync(endpoint);
            SaveSessionCookie(response);
            return response.IsSuccessStatusCode;
        }
    }
}