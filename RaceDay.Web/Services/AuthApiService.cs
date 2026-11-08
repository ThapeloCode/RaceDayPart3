using System.Net.Http.Json;
using RaceDay.Web.DTOs;

namespace RaceDay.Web.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _client;

        public AuthApiService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("RaceDayAPI");
        }

        public async Task<UserSessionDto?> LoginAsync(LoginDto loginDto)
        {
            var response = await _client.PostAsJsonAsync("auth/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserSessionDto>();
            }
            return null;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _client.PostAsJsonAsync("auth/register", registerDto);
            return response.IsSuccessStatusCode;
        }
    }
}