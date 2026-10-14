using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Helpers;
using RaceDay.API.Models;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Organiser profile management
    public class OrganisersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public OrganisersController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/organisers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Organiser>> GetOrganiser(int id)
        {
            if (HttpContext.Session.GetString("Role") == null)
                return Unauthorized("You must be logged in.");

            var org = await _context.Organisers.FindAsync(id);
            if (org == null) return NotFound("Organiser does not exist.");

            return Ok(org);
        }

        // PUT: api/organisers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganiser(int id, Organiser org)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update their details.");

            if (id != org.OrganiserID) return BadRequest("Invalid data.");

            if (HttpContext.Session.GetInt32("UserId") != id)
                return StatusCode(403, "You can only update your own account.");

            var existing = await _context.Organisers.FindAsync(id);
            if (existing == null) return NotFound("Organiser does not exist.");

            existing.Name = org.Name;
            existing.Email = org.Email;
            existing.Phone = org.Phone;

            if (!string.IsNullOrEmpty(org.PasswordHash))
                existing.PasswordHash = PasswordHasher.HashPassword(org.PasswordHash);

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/organisers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganiser(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser" && role != "Admin")
                return Unauthorized("Only Organisers or Admins can delete.");

            var org = await _context.Organisers.FindAsync(id);
            if (org == null) return NotFound("Organiser does not exist.");

            _context.Organisers.Remove(org);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
