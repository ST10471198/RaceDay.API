using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTO;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ApiRegisterDto req)
        {
            if (await _context.Organisers.AnyAsync(o => o.Email == req.Email) ||
                await _context.Participants.AnyAsync(p => p.Email == req.Email))
                return BadRequest("Email already exists.");

            if (req.Role == "Organiser")
            {
                var organiser = new Organiser
                {
                    Name = req.Name,
                    Email = req.Email,
                    Phone = req.Phone ?? "",
                    PasswordHash = PasswordHasher.HashPassword(req.Password),
                    CreatedAt = DateTime.Now
                };
                _context.Organisers.Add(organiser);
            }
            else
            {
                DateTime dob = new DateTime(2000, 1, 1);
                if (!string.IsNullOrEmpty(req.DateOfBirth)) DateTime.TryParse(req.DateOfBirth, out dob);

                var participant = new Participant
                {
                    Name = req.Name,
                    Email = req.Email,
                    PasswordHash = PasswordHasher.HashPassword(req.Password),
                    DateOfBirth = dob,
                    IdNumber = req.IdNumber ?? "N/A",
                    EmergencyContact = req.EmergencyContact ?? "N/A",
                    CreatedAt = DateTime.Now
                };
                _context.Participants.Add(participant);
            }

            await _context.SaveChangesAsync();
            return StatusCode(201, new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiLoginDto req)
        {
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.Email == req.Email);
            if (organiser != null && PasswordHasher.VerifyPassword(req.Password, organiser.PasswordHash))
            {
                HttpContext.Session.SetInt32("UserId", organiser.OrganiserID);
                HttpContext.Session.SetString("Role", "Organiser");
                return Ok(new { Id = organiser.OrganiserID, Name = organiser.Name, Email = organiser.Email, Role = "Organiser" });
            }

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.Email == req.Email);
            if (participant != null && PasswordHasher.VerifyPassword(req.Password, participant.PasswordHash))
            {
                HttpContext.Session.SetInt32("UserId", participant.ParticipantID);
                HttpContext.Session.SetString("Role", "Participant");
                return Ok(new { Id = participant.ParticipantID, Name = participant.Name, Email = participant.Email, Role = "Participant" });
            }

            return Unauthorized("Invalid credentials.");
        }
    }
}