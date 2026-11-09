using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using RaceDay.Web.DTOs;

namespace RaceDay.Web.Services
{
    public class ParticipantApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParticipantApiService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = clientFactory.CreateClient("RaceDayAPI");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachBearerToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            var response = await _client.GetAsync("events");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventDto>>() ?? new List<EventDto>();
            }
            return new List<EventDto>();
        }

        public async Task<List<CategoryDto>> GetEventCategoriesAsync(int eventId)
        {
            var response = await _client.GetAsync($"categories/event/{eventId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
            }
            return new List<CategoryDto>();
        }

        public async Task<bool> EnrolAsync(int userId, int categoryId)
        {
            AttachBearerToken();
            var payload = new EnrolmentRequestDto
            {
                UserId = userId,
                CategoryId = categoryId
            };

            var response = await _client.PostAsJsonAsync("enrolments", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ParticipantEnrolmentDto>> GetParticipantEnrolmentsAsync(int userId)
        {
            AttachBearerToken();
            var response = await _client.GetAsync($"enrolments/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ParticipantEnrolmentDto>>() ?? new List<ParticipantEnrolmentDto>();
            }
            return new List<ParticipantEnrolmentDto>();
        }

        public async Task<List<ParticipantResultDto>> GetParticipantResultsAsync(int userId)
        {
            AttachBearerToken();
            var response = await _client.GetAsync($"results/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ParticipantResultDto>>() ?? new List<ParticipantResultDto>();
            }
            return new List<ParticipantResultDto>();
        }
    }
}