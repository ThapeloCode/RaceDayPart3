using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RaceDay.Web.DTOs;

namespace RaceDay.Web.Services
{
    public class OrganiserApiService
    {
        private readonly HttpClient _client;

        public OrganiserApiService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("RaceDayAPI");
        }

        public async Task<List<EventDto>> GetOrganiserEventsAsync(int organiserId)
        {
            var response = await _client.GetAsync($"api/events/organiser/{organiserId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventDto>>() ?? new List<EventDto>();
            }
            return new List<EventDto>();
        }

        public async Task<bool> CreateEventAsync(CreateEventDto dto, int organiserId)
        {
            // Mapped payload matching backend EF Core database schema
            var payload = new
            {
                EventName = dto.Name,
                EventDate = dto.Date,
                EventType = dto.EventType,
                Location = dto.Location,
                OrganiserId = organiserId
            };

            var response = await _client.PostAsJsonAsync("api/events", payload);

            if (!response.IsSuccessStatusCode)
            {
                // Logs the status code and API error details to your terminal output
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[API ERROR] Status: {response.StatusCode} | Details: {errorBody}");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var response = await _client.DeleteAsync($"api/events/{eventId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CategoryDto>> GetEventCategoriesAsync(int eventId)
        {
            var response = await _client.GetAsync($"api/events/{eventId}/categories");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
            }
            return new List<CategoryDto>();
        }

        public async Task<bool> AddCategoryAsync(CreateCategoryDto dto)
        {
            var response = await _client.PostAsJsonAsync($"api/events/{dto.EventId}/categories", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<EventEnrolmentDto>> GetEventEnrolmentsAsync(int eventId)
        {
            var response = await _client.GetAsync($"api/events/{eventId}/enrolments");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventEnrolmentDto>>() ?? new List<EventEnrolmentDto>();
            }
            return new List<EventEnrolmentDto>();
        }
    }
}