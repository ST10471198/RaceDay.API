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
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            var (isSuccess, user) = await _apiService.PostAsync<AuthUserResponse>("api/Auth/login", model);

            if (!isSuccess || user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect based on role
            if (user.Role == "Organiser")
                return RedirectToAction("Dashboard", "Organiser");
            else
                return RedirectToAction("Index", "Participant");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            var (isSuccess, _) = await _apiService.PostAsync<object>("api/Auth/register", model);

            if (!isSuccess)
            {
                ModelState.AddModelError("", "Registration failed. Email might exist.");
                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}