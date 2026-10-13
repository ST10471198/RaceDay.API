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
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register/organiser
        [HttpPost("register/organiser")]
        public async Task<IActionResult> RegisterOrganiser([FromBody] Organiser org)
        {
            if (org == null || string.IsNullOrEmpty(org.Email) || string.IsNullOrEmpty(org.PasswordHash))
                return BadRequest("Invalid or missing details.");

            if (await _context.Organisers.AnyAsync(o => o.Email == org.Email))
                return Conflict("Email already exists.");

            org.PasswordHash = PasswordHasher.HashPassword(org.PasswordHash);
            _context.Organisers.Add(org);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Organiser created successfully.");
        }

        // POST: api/auth/register/participant
        [HttpPost("register/participant")]
        public async Task<IActionResult> RegisterParticipant([FromBody] Participant p)
        {
            if (p == null || string.IsNullOrEmpty(p.Email) || string.IsNullOrEmpty(p.PasswordHash))
                return BadRequest("Invalid or missing details.");

            if (await _context.Participants.AnyAsync(x => x.Email == p.Email))
                return Conflict("Participant already exists.");

            p.PasswordHash = PasswordHasher.HashPassword(p.PasswordHash);
            _context.Participants.Add(p);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Participant registered successfully.");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Invalid login details.");

            // Check Organiser first
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.Email == login.Email);
            if (organiser != null && PasswordHasher.VerifyPassword(login.Password, organiser.PasswordHash))
            {
                HttpContext.Session.SetInt32("UserId", organiser.OrganiserID);
                HttpContext.Session.SetString("Role", "Organiser");
                return Ok(new { role = "Organiser", id = organiser.OrganiserID, name = organiser.Name });
            }

            // Check Participant
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.Email == login.Email);
            if (participant != null && PasswordHasher.VerifyPassword(login.Password, participant.PasswordHash))
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

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}