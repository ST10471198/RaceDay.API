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
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromQuery] string email,
            [FromQuery] string password,
            [FromQuery] string firstName,
            [FromQuery] string surname,
            [FromQuery] string role,
            [FromQuery] DateTime? dateOfBirth = null,
            [FromQuery] string phoneNumber = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("Email and password are required.");

            if (role != "Organiser" && role != "Participant")
                return BadRequest("Role must be 'Organiser' or 'Participant'.");

            var fullName = $"{firstName} {surname}".Trim();

            if (role == "Organiser")
            {
                if (await _context.Organisers.AnyAsync(o => o.Email == email))
                    return Conflict("Email already exists.");

                var organiser = new Organiser
                {
                    Name = fullName,
                    Email = email,
                    Phone = phoneNumber ?? "",
                    PasswordHash = PasswordHasher.HashPassword(password),
                    CreatedAt = DateTime.Now
                };

                _context.Organisers.Add(organiser);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Organiser registered successfully.");
            }
            else
            {
                if (await _context.Participants.AnyAsync(p => p.Email == email))
                    return Conflict("Email already exists.");

                var participant = new Participant
                {
                    Name = fullName,
                    Email = email,
                    PasswordHash = PasswordHasher.HashPassword(password),
                    DateOfBirth = dateOfBirth ?? new DateTime(2000, 1, 1),
                    IdNumber = "N/A",
                    EmergencyContact = "N/A",
                    CreatedAt = DateTime.Now
                };

                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Participant registered successfully.");
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromQuery] string email,
            [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("Email and password are required.");

            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.Email == email);
            if (organiser != null && PasswordHasher.VerifyPassword(password, organiser.PasswordHash))
            {
                HttpContext.Session.SetInt32("UserId", organiser.OrganiserID);
                HttpContext.Session.SetString("Role", "Organiser");
                return Ok(new { role = "Organiser", id = organiser.OrganiserID, name = organiser.Name });
            }

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.Email == email);
            if (participant != null && PasswordHasher.VerifyPassword(password, participant.PasswordHash))
            {
                HttpContext.Session.SetInt32("UserId", participant.ParticipantID);
                HttpContext.Session.SetString("Role", "Participant");
                return Ok(new { role = "Participant", id = participant.ParticipantID, name = participant.Name });
            }

            return Unauthorized("Invalid credentials.");
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out successfully.");
        }
    }
}