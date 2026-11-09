using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.DTOs;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authApiService;

        public AccountController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var authResponse = await _authApiService.LoginAsync(model);

            if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
            {
                // Store authentication details and token in Session
                HttpContext.Session.SetString("JWToken", authResponse.Token);
                HttpContext.Session.SetInt32("UserId", authResponse.UserId);
                HttpContext.Session.SetString("UserRole", authResponse.Role ?? "Participant");
                HttpContext.Session.SetString("Username", model.Email);

                // Redirect based on role
                if (authResponse.Role?.Equals("Organiser", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return RedirectToAction("Index", "Organiser");
                }

                return RedirectToAction("Index", "Participant");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials and try again.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _authApiService.RegisterAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Email might already be in use.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}