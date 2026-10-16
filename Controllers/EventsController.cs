using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDayAPI.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _context.Events.Include(e => e.Categories).ToListAsync();
            if (events.Count == 0) return NotFound("No events found.");
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var ev = await _context.Events.Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventID == id);
            if (ev == null) return NotFound("Event does not exist.");
            return Ok(ev);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(
            [FromQuery] string name,
            [FromQuery] string description,
            [FromQuery] DateTime date,
            [FromQuery] string location,
            [FromQuery] int maxParticipants,
            [FromQuery] string status)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can create events.");

            var ev = new Event
            {
                Name = name,
                Description = description,
                Date = date,
                Location = location,
                MaxParticipants = maxParticipants,
                Status = status,
                OrganiserID = (int)HttpContext.Session.GetInt32("UserId")
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = ev.EventID }, ev);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromQuery] string name,
            [FromQuery] string description,
            [FromQuery] DateTime date,
            [FromQuery] string location,
            [FromQuery] int maxParticipants,
            [FromQuery] string status)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update events.");

            var existing = await _context.Events.FindAsync(id);
            if (existing == null) return NotFound("Event does not exist.");

            if (existing.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            existing.Name = name;
            existing.Description = description;
            existing.Date = date;
            existing.Location = location;
            existing.MaxParticipants = maxParticipants;
            existing.Status = status;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

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