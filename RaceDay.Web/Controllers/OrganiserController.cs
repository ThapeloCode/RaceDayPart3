using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ParticipantApiService _participantService;

        public OrganiserController(ParticipantApiService participantService)
        {
            _participantService = participantService;
        }

        private int? GetCurrentUserId() => HttpContext.Session.GetInt32("UserId");
        private string? GetUserRole() => HttpContext.Session.GetString("UserRole");
        private bool IsOrganiser() => GetUserRole()?.Equals("Organiser", StringComparison.OrdinalIgnoreCase) == true;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var userIdVal = GetCurrentUserId();
            if (userIdVal == null) return RedirectToAction("Login", "Account");
            var organiserId = userIdVal.Value;

            // Fetch all events and filter for the current organiser
            var allEvents = await _participantService.GetAllEventsAsync();
            var organiserEvents = allEvents.Where(e => e.OrganiserId == organiserId).ToList();

            // Populate the dashboard DTO matching your Index.cshtml model
            var dashboardDto = new RaceDay.Web.DTOs.OrganiserDashboardDto
            {
                TotalEvents = organiserEvents.Count,
                TotalEnrolments = organiserEvents.Sum(e => e.EnrolmentCount),
                UpcomingEvents = organiserEvents
                    .Where(e => e.Date > DateTime.Now)
                    .ToList(),
                AllEvents = organiserEvents
            };

            return View(dashboardDto);
        }

        [HttpGet]
        public async Task<IActionResult> Events()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var userIdVal = GetCurrentUserId();
            if (userIdVal == null) return RedirectToAction("Login", "Account");
            var organiserId = userIdVal.Value;

            // Fetch events created by this specific organiser from your API
            var events = await _participantService.GetAllEventsAsync();
            var organiserEvents = events.Where(e => e.OrganiserId == organiserId).ToList();

            return View(organiserEvents);
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(RaceDay.Web.DTOs.CreateEventDto model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var organiserIdVal = GetCurrentUserId();
            if (organiserIdVal == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map CreateEventDto fields to the EventDto expected by your API service client
            var eventDtoToSubmit = new RaceDay.Web.DTOs.EventDto
            {
                Name = model.Name,
                Description = model.Description,
                Date = model.Date,
                Location = model.Location,
                Distance = model.Distance,
                EventType = model.EventType,
                OrganiserId = organiserIdVal.Value
            };

            var success = await _participantService.CreateEventAsync(eventDtoToSubmit);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Failed to create the event via the API. Please try again.");
            return View(model);
        }
    }
}