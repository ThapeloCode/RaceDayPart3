using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ParticipantApiService _participantService;

        public ParticipantController(ParticipantApiService participantService)
        {
            _participantService = participantService;
        }

        private int? GetCurrentUserId() => HttpContext.Session.GetInt32("UserId");
        private string? GetUserRole() => HttpContext.Session.GetString("UserRole");

        private bool IsParticipant() =>
            GetUserRole()?.Equals("Participant", StringComparison.OrdinalIgnoreCase) == true;

        [HttpGet]
        public async Task<IActionResult> BrowseEvents()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var events = await _participantService.GetAllEventsAsync();
            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> EventDetails(int id)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var events = await _participantService.GetAllEventsAsync();
            var eventItem = events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null) return NotFound();

            var categories = await _participantService.GetEventCategoriesAsync(id);
            ViewBag.Categories = categories;

            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int categoryId, int eventId)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");

            var userId = GetCurrentUserId()!.Value;
            var success = await _participantService.EnrolAsync(userId, categoryId);

            if (success)
            {
                TempData["SuccessMessage"] = "Successfully enrolled for the event!";
                return RedirectToAction("MyEnrolments");
            }

            TempData["ErrorMessage"] = "Failed to enrol. You may already be enrolled in this category.";
            return RedirectToAction("EventDetails", new { id = eventId });
        }

        [HttpGet]
        public async Task<IActionResult> MyEnrolments()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");

            var userId = GetCurrentUserId()!.Value;
            var enrolments = await _participantService.GetParticipantEnrolmentsAsync(userId);
            return View(enrolments);
        }

        [HttpGet]
        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");

            var userId = GetCurrentUserId()!.Value;
            var results = await _participantService.GetParticipantResultsAsync(userId);
            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");

            var userId = GetCurrentUserId()!.Value;

            // You can fetch additional profile info here if your service supports it
            return View();
        }
    }
}