using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using RaceDayAPI.Data;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/5/categories  (Public)
        [HttpGet("events/{eventId}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesForEvent(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var categories = await _context.Categories
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            if (categories.Count == 0) return NotFound("No categories found.");
            return Ok(categories);
        }

        // GET: api/categories/5  (Public)
        [HttpGet("categories/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound("Category does not exist.");
            return Ok(cat);
        }

        // POST: api/events/5/categories  (Organiser Only)
        [HttpPost("events/{eventId}/categories")]
        public async Task<ActionResult<Category>> CreateCategory(int eventId, Category cat)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can create categories.");

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            if (cat == null) return BadRequest("Invalid category information.");

            cat.EventID = eventId;
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            return StatusCode(201, cat);
        }

        // PUT: api/categories/5  (Organiser Only)
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category cat)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update categories.");

            if (id != cat.CategoryID) return BadRequest("Invalid information.");

            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return NotFound("Category does not exist.");

            var ev = await _context.Events.FindAsync(existing.EventID);
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            existing.Name = cat.Name;
            existing.DistanceKM = cat.DistanceKM;
            existing.StartTime = cat.StartTime;
            existing.AgeMin = cat.AgeMin;
            existing.AgeMax = cat.AgeMax;
            existing.EntryFee = cat.EntryFee;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/categories/5  (Organiser Only)
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can delete categories.");

            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound("Category does not exist.");

            var ev = await _context.Events.FindAsync(cat.EventID);
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
