using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpPost("events/{eventId}/categories/{categoryId}/enrol")]
        public async Task<ActionResult<Result>> EnrolParticipant(int eventId, int categoryId)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return Unauthorized("Only Participants can enrol in events.");

            var participantId = HttpContext.Session.GetInt32("UserId");
            if (participantId == null) return Unauthorized("Session expired.");

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var cat = await _context.Categories.FindAsync(categoryId);
            if (cat == null) return NotFound("Category does not exist.");

            var alreadyEnrolled = await _context.Results
                .AnyAsync(r => r.EventID == eventId && r.ParticipantID == participantId);
            if (alreadyEnrolled)
                return Conflict("You are already enrolled in this event.");

            var enrolment = new Result
            {
                EventID = eventId,
                CategoryID = categoryId,
                ParticipantID = participantId.Value,
                Status = "Registered"
            };

            _context.Results.Add(enrolment);
            await _context.SaveChangesAsync();
            return StatusCode(201, enrolment);
        }

        [HttpPost("events/{eventId}/categories/{categoryId}/results")]
        public async Task<ActionResult<Result>> RecordResult(
            int eventId,
            int categoryId,
            [FromQuery] int participantId,
            [FromQuery] TimeSpan? finishTime,
            [FromQuery] int? position,
            [FromQuery] string status)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can record results.");

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var cat = await _context.Categories.FindAsync(categoryId);
            if (cat == null) return NotFound("Category does not exist.");

            var participant = await _context.Participants.FindAsync(participantId);
            if (participant == null) return NotFound("Participant does not exist.");

            var result = new Result
            {
                EventID = eventId,
                CategoryID = categoryId,
                ParticipantID = participantId,
                FinishTime = finishTime,
                Position = position,
                Status = status
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return StatusCode(201, result);
        }

        [HttpGet("events/{eventId}/results")]
        public async Task<ActionResult<IEnumerable<Result>>> GetResultsForEvent(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var results = await _context.Results
                .Where(r => r.EventID == eventId)
                .Include(r => r.Participant)
                .Include(r => r.Category)
                .ToListAsync();

            if (results.Count == 0) return NotFound("No results found.");
            return Ok(results);
        }

        [HttpGet("categories/{categoryId}/results")]
        public async Task<ActionResult<IEnumerable<Result>>> GetResultsForCategory(int categoryId)
        {
            var cat = await _context.Categories.FindAsync(categoryId);
            if (cat == null) return NotFound("Category does not exist.");

            var results = await _context.Results
                .Where(r => r.CategoryID == categoryId)
                .Include(r => r.Participant)
                .ToListAsync();

            if (results.Count == 0) return NotFound("No results found.");
            return Ok(results);
        }

        [HttpGet("participants/{participantId}/results")]
        public async Task<ActionResult<IEnumerable<Result>>> GetParticipantResults(int participantId)
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (role == "Participant" && userId != participantId)
                return StatusCode(403, "You can only view your own results.");

            if (role != "Participant" && role != "Organiser" && role != "Admin")
                return Unauthorized("You must be logged in.");

            var results = await _context.Results
                .Where(r => r.ParticipantID == participantId)
                .Include(r => r.Event)
                .Include(r => r.Category)
                .ToListAsync();

            if (results.Count == 0) return NotFound("No results found.");
            return Ok(results);
        }

        [HttpGet("results/{id}")]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            var r = await _context.Results
                .Include(x => x.Participant)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.ResultID == id);

            if (r == null) return NotFound("Result does not exist.");
            return Ok(r);
        }

        [HttpPut("results/{id}")]
        public async Task<IActionResult> UpdateResult(
            int id,
            [FromQuery] TimeSpan? finishTime,
            [FromQuery] int? position,
            [FromQuery] string status)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update results.");

            var existing = await _context.Results.FindAsync(id);
            if (existing == null) return NotFound("Result does not exist.");

            existing.FinishTime = finishTime;
            existing.Position = position;
            existing.Status = status;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("results/{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can delete results.");

            var r = await _context.Results.FindAsync(id);
            if (r == null) return NotFound("Result does not exist.");

            _context.Results.Remove(r);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}