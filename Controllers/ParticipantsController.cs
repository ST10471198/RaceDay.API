using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Helpers;
using RaceDay.API.Models;
using RaceDayAPI.Data;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ParticipantsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/participants  (Public — register)
        [HttpPost]
        public async Task<ActionResult<Participant>> RegisterParticipant(Participant p)
        {
            if (p == null || string.IsNullOrEmpty(p.Email))
                return BadRequest("Invalid information.");

            if (await _context.Participants.AnyAsync(x => x.Email == p.Email))
                return Conflict("Participant already exists.");

            p.PasswordHash = PasswordHasher.HashPassword(p.PasswordHash);
            _context.Participants.Add(p);
            await _context.SaveChangesAsync();

            return StatusCode(201, p);
        }

        // GET: api/participants/5  (Participant/Admin)
        [HttpGet("{id}")]
        public async Task<ActionResult<Participant>> GetParticipant(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant" && role != "Admin")
                return Unauthorized("You must be logged in as a Participant.");

            var p = await _context.Participants.FindAsync(id);
            if (p == null) return NotFound("Participant does not exist.");

            return Ok(p);
        }

        // PUT: api/participants/5  (Participant)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParticipant(int id, Participant p)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return Unauthorized("Only Participants can update their profile.");

            if (HttpContext.Session.GetInt32("UserId") != id)
                return StatusCode(403, "You can only update your own account.");

            var existing = await _context.Participants.FindAsync(id);
            if (existing == null) return NotFound("Participant does not exist.");

            existing.Name = p.Name;
            existing.Email = p.Email;
            existing.DateOfBirth = p.DateOfBirth;
            existing.EmergencyContact = p.EmergencyContact;

            if (!string.IsNullOrEmpty(p.PasswordHash))
                existing.PasswordHash = PasswordHasher.HashPassword(p.PasswordHash);

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/participants/5  (Participant/Admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant" && role != "Admin")
                return Unauthorized("Only Participants or Admins can delete.");

            if (role == "Participant" && HttpContext.Session.GetInt32("UserId") != id)
                return StatusCode(403, "You can only delete your own account.");

            var p = await _context.Participants.FindAsync(id);
            if (p == null) return NotFound("Participant does not exist.");

            _context.Participants.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
