using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if the user is already logged in
            var role = HttpContext.Session.GetString("Role");

            if (role == "Organiser")
            {
                return RedirectToAction("Dashboard", "Organiser");
            }
            else if (role == "Participant")
            {
                return RedirectToAction("Index", "Participant");
            }

            // If not logged in, show the login page
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (isSuccess, user) = await _apiService.PostAsync<AuthUserResponse>("api/Auth/login", model);

            if (!isSuccess || user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Store user details in the MVC Session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect to the correct dashboard
            if (user.Role == "Organiser")
                return RedirectToAction("Dashboard", "Organiser");
            else
                return RedirectToAction("Index", "Participant");
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Redirect away from register page if already logged in
            var role = HttpContext.Session.GetString("Role");
            if (role == "Organiser") return RedirectToAction("Dashboard", "Organiser");
            if (role == "Participant") return RedirectToAction("Index", "Participant");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (isSuccess, _) = await _apiService.PostAsync<object>("api/Auth/register", model);

            if (!isSuccess)
            {
                ModelState.AddModelError("", "Registration failed. Email might already be in use.");
                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
} 