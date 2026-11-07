using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Services;
using RaceDayAPI.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly IBlobStorageService _blobService;

        public EventsController(RaceDayDbContext context, IBlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _context.Events.Include(e => e.Categories).ToListAsync();
            if (events.Count == 0) return NotFound("No events found.");
            return Ok(events);
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var ev = await _context.Events.Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (ev == null) return NotFound("Event does not exist.");
            return Ok(ev);
        }

        // POST: api/Events
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Event>> CreateEvent(
            [FromForm] string name,
            [FromForm] string description,
            [FromForm] DateTime date,
            [FromForm] string location,
            [FromForm] int maxParticipants,
            [FromForm] string status,
            IFormFile? bannerImage) // <-- NO [FromForm] HERE
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can create events.");

            string? imageUrl = null;

            if (bannerImage != null && bannerImage.Length > 0)
            {
                imageUrl = await _blobService.UploadFileAsync(bannerImage);
            }

            var ev = new Event
            {
                Name = name,
                Description = description,
                Date = date,
                Location = location,
                MaxParticipants = maxParticipants,
                Status = status,
                BannerImageUrl = imageUrl,
                OrganiserID = HttpContext.Session.GetInt32("UserId") ?? 0
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = ev.EventID }, ev);
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromForm] string name,
            [FromForm] string description,
            [FromForm] DateTime date,
            [FromForm] string location,
            [FromForm] int maxParticipants,
            [FromForm] string status,
            IFormFile? bannerImage) // <-- NO [FromForm] HERE
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update events.");

            var existing = await _context.Events.FindAsync(id);
            if (existing == null) return NotFound("Event does not exist.");

            if (existing.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            if (bannerImage != null && bannerImage.Length > 0)
            {
                existing.BannerImageUrl = await _blobService.UploadFileAsync(bannerImage);
            }

            existing.Name = name;
            existing.Description = description;
            existing.Date = date;
            existing.Location = location;
            existing.MaxParticipants = maxParticipants;
            existing.Status = status;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can delete events.");

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound("Event does not exist.");

            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}