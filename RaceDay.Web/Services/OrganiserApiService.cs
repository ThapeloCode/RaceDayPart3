using System.Net.Http.Json;
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
            var response = await _client.GetAsync($"events/organiser/{organiserId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventDto>>() ?? new List<EventDto>();
            }
            return new List<EventDto>();
        }

        public async Task<bool> CreateEventAsync(CreateEventDto dto, int organiserId)
        {
            var payload = new
            {
                dto.Name,
                dto.Description,
                dto.Date,
                dto.Location,
                dto.Distance,
                dto.EventType,
                OrganiserId = organiserId
            };

            var response = await _client.PostAsJsonAsync("events", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var response = await _client.DeleteAsync($"events/{eventId}");
            return response.IsSuccessStatusCode;
        }
    }
}