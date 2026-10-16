using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Helpers;

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

        [HttpPost]
        public async Task<ActionResult<Participant>> RegisterParticipant(
            [FromQuery] string name,
            [FromQuery] string email,
            [FromQuery] string password,
            [FromQuery] DateTime dateOfBirth,
            [FromQuery] string idNumber,
            [FromQuery] string emergencyContact)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("Invalid information.");

            if (await _context.Participants.AnyAsync(x => x.Email == email))
                return Conflict("Participant already exists.");

            var p = new Participant
            {
                Name = name,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(password),
                DateOfBirth = dateOfBirth,
                IdNumber = idNumber,
                EmergencyContact = emergencyContact,
                CreatedAt = DateTime.Now
            };

            _context.Participants.Add(p);
            await _context.SaveChangesAsync();
            return StatusCode(201, p);
        }

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParticipant(
            int id,
            [FromQuery] string name,
            [FromQuery] string email,
            [FromQuery] DateTime dateOfBirth,
            [FromQuery] string emergencyContact)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return Unauthorized("Only Participants can update their profile.");

            if (HttpContext.Session.GetInt32("UserId") != id)
                return StatusCode(403, "You can only update your own account.");

            var existing = await _context.Participants.FindAsync(id);
            if (existing == null) return NotFound("Participant does not exist.");

            existing.Name = name;
            existing.Email = email;
            existing.DateOfBirth = dateOfBirth;
            existing.EmergencyContact = emergencyContact;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

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