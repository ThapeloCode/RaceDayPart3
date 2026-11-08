using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly OrganiserApiService _organiserService;

        public OrganiserController(OrganiserApiService organiserService)
        {
            _organiserService = organiserService;
        }

        private int? GetCurrentUserId() => HttpContext.Session.GetInt32("UserId");
        private string? GetUserRole() => HttpContext.Session.GetString("UserRole");

        private bool IsOrganiser() =>
            GetUserRole()?.Equals("Organiser", StringComparison.OrdinalIgnoreCase) == true;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var userId = GetCurrentUserId()!.Value;
            var events = await _organiserService.GetOrganiserEventsAsync(userId);

            var dashboard = new OrganiserDashboardDto
            {
                TotalEvents = events.Count,
                TotalEnrolments = events.Sum(e => e.EnrolmentCount),
                UpcomingEvents = events.Where(e => e.Date >= DateTime.Now).OrderBy(e => e.Date).ToList(),
                AllEvents = events
            };

            return View(dashboard);
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            return View(new CreateEventDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventDto model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid) return View(model);

            var userId = GetCurrentUserId()!.Value;
            var success = await _organiserService.CreateEventAsync(model, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create event. Please try again.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            await _organiserService.DeleteEventAsync(id);
            TempData["SuccessMessage"] = "Event deleted.";
            return RedirectToAction("Index");
        }
    }
}