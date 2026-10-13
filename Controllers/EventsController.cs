using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using RaceDayAPI.Data;
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

        // GET: api/events  (Public)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _context.Events
                .Include(e => e.Categories)
                .ToListAsync();

            if (events == null || events.Count == 0)
                return NotFound("No events found.");

            return Ok(events);
        }

        // GET: api/events/5  (Public)
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (ev == null) return NotFound("Event does not exist.");
            return Ok(ev);
        }

        // POST: api/events  (Organiser Only)
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event ev)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can create events.");

            if (ev == null) return BadRequest("Invalid event information.");

            ev.OrganiserID = (int)HttpContext.Session.GetInt32("UserId");
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = ev.EventID }, ev);
        }

        // PUT: api/events/5  (Organiser Only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, Event ev)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update events.");

            if (id != ev.EventID) return BadRequest("Invalid information.");

            var existing = await _context.Events.FindAsync(id);
            if (existing == null) return NotFound("Event does not exist.");

            if (existing.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            existing.Description = ev.Description;
            existing.Date = ev.Date;
            existing.Location = ev.Location;
            existing.MaxParticipants = ev.MaxParticipants;
            existing.Status = ev.Status;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/events/5  (Organiser Only)
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








