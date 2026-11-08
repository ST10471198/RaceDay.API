using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly IApiService _apiService;

        public OrganiserController(IApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("Role") == "Organiser";
        }

        // --- DASHBOARD ---
        public async Task<IActionResult> Dashboard()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            var allEvents = await _apiService.GetAsync<List<EventViewModel>>("api/Events");
            var myEvents = allEvents?.Where(e => e.OrganiserID == userId).ToList() ?? new List<EventViewModel>();

            // Calculate Total Enrolments for each event
            var enrolmentCounts = new Dictionary<int, int>();
            foreach (var ev in myEvents)
            {
                var enrolments = await _apiService.GetAsync<List<EnrolmentViewModel>>($"api/Events/{ev.EventID}/enrolments");
                enrolmentCounts[ev.EventID] = enrolments?.Count ?? 0;
            }
            ViewBag.EnrolmentCounts = enrolmentCounts;

            return View(myEvents);
        }

        // --- CREATE EVENT ---
        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventModel model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Name), "name");
            content.Add(new StringContent(model.Description ?? ""), "description");
            content.Add(new StringContent(model.Date.ToString("yyyy-MM-ddTHH:mm:ss")), "date");
            content.Add(new StringContent(model.Location), "location");
            content.Add(new StringContent(model.MaxParticipants.ToString()), "maxParticipants");
            content.Add(new StringContent(model.Status), "status");

            if (model.BannerImage != null)
            {
                var fileStream = model.BannerImage.OpenReadStream();
                content.Add(new StreamContent(fileStream), "bannerImage", model.BannerImage.FileName);
            }

            var (isSuccess, _) = await _apiService.PostFileAsync<object>("api/Events", content);
            if (isSuccess) return RedirectToAction(nameof(Dashboard));

            ModelState.AddModelError("", "Failed to create event.");
            return View(model);
        }

        // --- EDIT EVENT ---
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            var ev = await _apiService.GetAsync<EventViewModel>($"api/Events/{id}");
            if (ev == null) return NotFound();

            var model = new CreateEventModel
            {
                Name = ev.Name,
                Description = ev.Description,
                Date = ev.Date,
                Location = ev.Location,
                MaxParticipants = ev.MaxParticipants,
                Status = ev.Status
            };

            ViewBag.EventId = id;
            ViewBag.ExistingImage = ev.BannerImageUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(int id, CreateEventModel model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Name), "name");
            content.Add(new StringContent(model.Description ?? ""), "description");
            content.Add(new StringContent(model.Date.ToString("yyyy-MM-ddTHH:mm:ss")), "date");
            content.Add(new StringContent(model.Location), "location");
            content.Add(new StringContent(model.MaxParticipants.ToString()), "maxParticipants");
            content.Add(new StringContent(model.Status), "status");

            if (model.BannerImage != null)
            {
                var fileStream = model.BannerImage.OpenReadStream();
                content.Add(new StreamContent(fileStream), "bannerImage", model.BannerImage.FileName);
            }

            var (isSuccess, _) = await _apiService.PutFileAsync<object>($"api/Events/{id}", content);

            if (isSuccess) return RedirectToAction(nameof(Dashboard));

            ModelState.AddModelError("", "Failed to update event.");
            ViewBag.EventId = id;
            ViewBag.ExistingImage = model.BannerImage?.FileName;
            return View(model);
        }

        // --- DELETE EVENT ---
        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            await _apiService.DeleteAsync($"api/Events/{id}");
            return RedirectToAction(nameof(Dashboard));
        }

        // --- MANAGE CATEGORIES ---
        [HttpGet]
        public async Task<IActionResult> ManageCategories(int eventId)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            var ev = await _apiService.GetAsync<EventViewModel>($"api/Events/{eventId}");
            if (ev == null) return NotFound("Event not found.");
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            var (isSuccess, _) = await _apiService.PostAsync<object>($"api/Events/{model.EventID}/categories", model);
            if (!isSuccess) ModelState.AddModelError("", "Failed to add category.");
            return RedirectToAction(nameof(ManageCategories), new { eventId = model.EventID });
        }

        // --- ENROLMENTS ---
        [HttpGet]
        public async Task<IActionResult> Enrolments(int eventId)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            var ev = await _apiService.GetAsync<EventViewModel>($"api/Events/{eventId}");
            ViewBag.EventName = ev?.Name ?? "Event";
            ViewBag.EventId = eventId;
            var enrolments = await _apiService.GetAsync<List<EnrolmentViewModel>>($"api/Events/{eventId}/enrolments");
            return View(enrolments ?? new List<EnrolmentViewModel>());
        }

        // --- RESULTS ENTRY ---
        [HttpGet]
        public async Task<IActionResult> EnterResults(int eventId)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            var ev = await _apiService.GetAsync<EventViewModel>($"api/Events/{eventId}");
            ViewBag.EventName = ev?.Name ?? "Event";
            ViewBag.EventId = eventId;
            var enrolments = await _apiService.GetAsync<List<EnrolmentViewModel>>($"api/Events/{eventId}/enrolments");
            return View(enrolments ?? new List<EnrolmentViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> SaveResults(List<EnrolmentViewModel> results, int eventId)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            var updateData = results.Select(r => new { ResultID = r.ResultID, FinishTime = r.FinishTime, Position = r.Position }).ToList();
            var (isSuccess, _) = await _apiService.PostAsync<object>("api/Results/batch", updateData);

            if (isSuccess) return RedirectToAction(nameof(Enrolments), new { eventId = eventId });

            ModelState.AddModelError("", "Failed to save results.");
            ViewBag.EventId = eventId;
            return View("EnterResults", results);
        }
    }
}