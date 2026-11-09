using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.DTOs;

namespace RaceDay.Web.Services
{
    public class ParticipantApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParticipantApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            // Fallback base address to prevent InvalidOperationException if not injected via DI
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5289/");
            }
        }

        private void AttachToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            AttachToken();
            try
            {
                var events = await _httpClient.GetFromJsonAsync<List<EventDto>>("api/events");
                return events ?? new List<EventDto>();
            }
            catch (Exception)
            {
                return new List<EventDto>();
            }
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            AttachToken();
            try
            {
                return await _httpClient.GetFromJsonAsync<EventDto>($"api/events/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateEventAsync(EventDto eventDto)
        {
            AttachToken();
            var response = await _httpClient.PostAsJsonAsync("api/events", eventDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            AttachToken();
            var response = await _httpClient.DeleteAsync($"api/events/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CategoryDto>> GetEventCategoriesAsync(int eventId)
        {
            AttachToken();
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>($"api/categories/event/{eventId}");
                return categories ?? new List<CategoryDto>();
            }
            catch (Exception)
            {
                return new List<CategoryDto>();
            }
        }

        public async Task<bool> EnrolAsync(int eventId, int categoryId, int userId = 0)
        {
            AttachToken();

            if (userId <= 0)
            {
                var sessionUserId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
                if (sessionUserId.HasValue)
                {
                    userId = sessionUserId.Value;
                }
            }

            var enrolmentPayload = new { EventId = eventId, CategoryId = categoryId, UserId = userId };
            var response = await _httpClient.PostAsJsonAsync("api/enrolments", enrolmentPayload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<EventEnrolmentDto>> GetParticipantEnrolmentsAsync(int userId)
        {
            AttachToken();
            try
            {
                var enrolments = await _httpClient.GetFromJsonAsync<List<EventEnrolmentDto>>($"api/enrolments/user/{userId}");
                return enrolments ?? new List<EventEnrolmentDto>();
            }
            catch (Exception)
            {
                return new List<EventEnrolmentDto>();
            }
        }

        public async Task<List<object>> GetParticipantResultsAsync(int userId)
        {
            AttachToken();
            try
            {
                var results = await _httpClient.GetFromJsonAsync<List<object>>($"api/results/user/{userId}");
                return results ?? new List<object>();
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }
    }
}