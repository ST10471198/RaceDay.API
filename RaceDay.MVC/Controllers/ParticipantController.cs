using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Text;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IApiService _apiService;

        public ParticipantController(IApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsParticipant()
        {
            return HttpContext.Session.GetString("Role") == "Participant";
        }

        private int GetUserId()
        {
            return int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
        }

        public async Task<IActionResult> Index()
        {
            var allEvents = await _apiService.GetAsync<List<EventViewModel>>("api/Events");
            var upcomingEvents = allEvents?
                .Where(e => e.Status == "Open" || e.Status == "Upcoming")
                .OrderBy(e => e.Date)
                .ToList() ?? new List<EventViewModel>();

            return View(upcomingEvents);
        }

        public async Task<IActionResult> EventDetails(int id)
        {
            var ev = await _apiService.GetAsync<EventViewModel>($"api/Events/{id}");
            if (ev == null) return NotFound("Event not found.");
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int EventID, int CategoryID) // Fixed parameter names
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var dto = new
            {
                EventID = EventID,
                ParticipantID = GetUserId(),
                CategoryID = CategoryID
            };

            var (isSuccess, _) = await _apiService.PostAsync<object>("api/Results", dto);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = "You have successfully enrolled!";
                return RedirectToAction(nameof(MyEvents));
            }

            TempData["ErrorMessage"] = "Failed to enrol. You might already be registered.";
            return RedirectToAction(nameof(EventDetails), new { id = EventID });
        }

        public async Task<IActionResult> MyEvents()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var allEvents = await _apiService.GetAsync<List<EventViewModel>>("api/Events");
            var myEvents = new List<EventViewModel>();

            if (allEvents != null)
            {
                foreach (var ev in allEvents)
                {
                    var enrolments = await _apiService.GetAsync<List<EnrolmentViewModel>>($"api/Events/{ev.EventID}/enrolments");
                    var myEnrolment = enrolments?.FirstOrDefault(e => e.ParticipantID == GetUserId());
                    if (myEnrolment != null)
                    {
                        myEvents.Add(ev);
                    }
                }
            }

            return View(myEvents);
        }

        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var allEvents = await _apiService.GetAsync<List<EventViewModel>>("api/Events");
            var myResults = new List<EnrolmentViewModel>();

            if (allEvents != null)
            {
                foreach (var ev in allEvents)
                {
                    var enrolments = await _apiService.GetAsync<List<EnrolmentViewModel>>($"api/Events/{ev.EventID}/enrolments");
                    var myEnrolment = enrolments?.FirstOrDefault(e => e.ParticipantID == GetUserId());
                    if (myEnrolment != null)
                    {
                        myResults.Add(myEnrolment);
                    }
                }
            }

            return View(myResults);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var profile = await _apiService.GetAsync<ParticipantProfileViewModel>($"api/Participants/{GetUserId()}");
            if (profile == null) return NotFound();

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ParticipantProfileViewModel model)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Name), "name");
            content.Add(new StringContent(model.EmergencyContact ?? ""), "emergencyContact");
            content.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd")), "dateOfBirth");

            if (model.ProfilePicture != null)
            {
                var stream = model.ProfilePicture.OpenReadStream();
                content.Add(new StreamContent(stream), "profilePicture", model.ProfilePicture.FileName);
            }

            var (isSuccess, _) = await _apiService.PutFileAsync<object>($"api/Participants/{GetUserId()}", content);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            ModelState.AddModelError("", "Failed to update profile.");
            return View(model);
        }
    }
}