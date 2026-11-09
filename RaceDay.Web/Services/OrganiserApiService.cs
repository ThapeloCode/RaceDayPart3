using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using RaceDay.Web.DTOs;

namespace RaceDay.Web.Services
{
    public class OrganiserApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrganiserApiService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<EventDto>> GetOrganiserEventsAsync(int organiserId)
        {
            AttachBearerToken();
            var response = await _client.GetAsync($"events/organiser/{organiserId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventDto>>() ?? new List<EventDto>();
            }
            return new List<EventDto>();
        }

        public async Task<bool> CreateEventAsync(CreateEventDto createDto, int organiserId)
        {
            AttachBearerToken();
            var payload = new
            {
                EventName = createDto.Name,
                Description = createDto.Description,
                EventType = createDto.EventType,
                Location = createDto.Location,
                Distance = createDto.Distance,
                EventDate = createDto.Date,
                OrganiserId = organiserId
            };

            var response = await _client.PostAsJsonAsync("events", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            AttachBearerToken();
            var response = await _client.DeleteAsync($"events/{eventId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CategoryDto>> GetEventCategoriesAsync(int eventId)
        {
            AttachBearerToken();
            var response = await _client.GetAsync($"categories/event/{eventId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
            }
            return new List<CategoryDto>();
        }

        public async Task<bool> AddCategoryAsync(CreateCategoryDto categoryDto)
        {
            AttachBearerToken();
            var response = await _client.PostAsJsonAsync($"events/{categoryDto.EventId}/categories", categoryDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<EventEnrolmentDto>> GetEventEnrolmentsAsync(int eventId)
        {
            AttachBearerToken();
            var response = await _client.GetAsync($"enrolments/event/{eventId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventEnrolmentDto>>() ?? new List<EventEnrolmentDto>();
            }
            return new List<EventEnrolmentDto>();
        }
    }
}