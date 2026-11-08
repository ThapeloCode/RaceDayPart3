using System.Net.Http.Json;
using RaceDay.Web.DTOs;

namespace RaceDay.Web.Services
{
    public class ParticipantApiService
    {
        private readonly HttpClient _client;

        public ParticipantApiService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("RaceDayAPI");
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            var response = await _client.GetAsync("events");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventDto>>() ?? new();
            }
            return new();
        }

        public async Task<List<CategoryDto>> GetEventCategoriesAsync(int eventId)
        {
            var response = await _client.GetAsync($"events/{eventId}/categories");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
            }
            return new();
        }

        public async Task<bool> EnrolAsync(int userId, int categoryId)
        {
            var payload = new EnrolmentRequestDto { UserId = userId, CategoryId = categoryId };
            var response = await _client.PostAsJsonAsync("enrolments", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ParticipantEnrolmentDto>> GetParticipantEnrolmentsAsync(int userId)
        {
            var response = await _client.GetAsync($"enrolments/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ParticipantEnrolmentDto>>() ?? new();
            }
            return new();
        }

        public async Task<List<ParticipantResultDto>> GetParticipantResultsAsync(int userId)
        {
            var response = await _client.GetAsync($"results/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ParticipantResultDto>>() ?? new();
            }
            return new();
        }
    }
}