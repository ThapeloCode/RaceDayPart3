using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authService;

        public AccountController(AuthApiService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var userSession = await _authService.LoginAsync(model);
            if (userSession == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            // Save user details to Session
            HttpContext.Session.SetInt32("UserId", userSession.UserId);
            HttpContext.Session.SetString("UserName", userSession.FullName);
            HttpContext.Session.SetString("UserRole", userSession.Role);
            HttpContext.Session.SetString("AuthToken", userSession.Token);

            // Redirect based on role
            if (userSession.Role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Organiser");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _authService.RegisterAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Email may already be in use.");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}