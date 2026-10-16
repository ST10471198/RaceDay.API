using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Helpers;
using RaceDay.API.Models;
using RaceDayAPI.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public OrganisersController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Organiser>> GetOrganiser(int id)
        {
            if (HttpContext.Session.GetString("Role") == null)
                return Unauthorized("You must be logged in.");

            var org = await _context.Organisers.FindAsync(id);
            if (org == null) return NotFound("Organiser does not exist.");
            return Ok(org);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganiser(
            int id,
            [FromQuery] string name,
            [FromQuery] string email,
            [FromQuery] string phone,
            [FromQuery] string password = null)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update their details.");

            if (HttpContext.Session.GetInt32("UserId") != id)
                return StatusCode(403, "You can only update your own account.");

            var existing = await _context.Organisers.FindAsync(id);
            if (existing == null) return NotFound("Organiser does not exist.");

            existing.Name = name;
            existing.Email = email;
            existing.Phone = phone;

            if (!string.IsNullOrEmpty(password))
                existing.PasswordHash = PasswordHasher.HashPassword(password);

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

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