using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

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

        [HttpGet("events/{eventId}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesForEvent(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var categories = await _context.Categories.Where(c => c.EventID == eventId).ToListAsync();
            if (categories.Count == 0) return NotFound("No categories found.");
            return Ok(categories);
        }

        [HttpGet("categories/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound("Category does not exist.");
            return Ok(cat);
        }

        [HttpPost("events/{eventId}/categories")]
        public async Task<ActionResult<Category>> CreateCategory(
            int eventId,
            [FromQuery] string name,
            [FromQuery] double distanceKM,
            [FromQuery] TimeSpan startTime,
            [FromQuery] int ageMin,
            [FromQuery] int ageMax,
            [FromQuery] decimal entryFee)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can create categories.");

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            var cat = new Category
            {
                Name = name,
                DistanceKM = distanceKM,
                StartTime = startTime,
                AgeMin = ageMin,
                AgeMax = ageMax,
                EntryFee = entryFee,
                EventID = eventId
            };

            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            return StatusCode(201, cat);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromQuery] string name,
            [FromQuery] double distanceKM,
            [FromQuery] TimeSpan startTime,
            [FromQuery] int ageMin,
            [FromQuery] int ageMax,
            [FromQuery] decimal entryFee)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update categories.");

            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return NotFound("Category does not exist.");

            var ev = await _context.Events.FindAsync(existing.EventID);
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            existing.Name = name;
            existing.DistanceKM = distanceKM;
            existing.StartTime = startTime;
            existing.AgeMin = ageMin;
            existing.AgeMax = ageMax;
            existing.EntryFee = entryFee;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

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