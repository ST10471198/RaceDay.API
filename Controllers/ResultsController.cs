using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using RaceDayAPI.Data;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    //Manages enrolments and race results
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // ENROLMENT: Participant registers themselves into a category
        // POST: api/events/{eventId}/categories/{categoryId}/enrol
        // ============================================================
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

            // Prevent duplicate enrolment
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

        // ============================================================
        // RECORD RESULT: Organiser captures finish time & position
        // POST: api/events/{eventId}/categories/{categoryId}/results
        // ============================================================
        [HttpPost("events/{eventId}/categories/{categoryId}/results")]
        public async Task<ActionResult<Result>> RecordResult(int eventId, int categoryId, Result result)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can record results.");

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var cat = await _context.Categories.FindAsync(categoryId);
            if (cat == null) return NotFound("Category does not exist.");

            var participant = await _context.Participants.FindAsync(result.ParticipantID);
            if (participant == null) return NotFound("Participant does not exist.");

            if (result == null) return BadRequest("Invalid result information.");

            result.EventID = eventId;
            result.CategoryID = categoryId;
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, result);
        }

        // ============================================================
        // ORGANISER VIEW: All enrolments/results for an event
        // GET: api/events/{eventId}/results
        // ============================================================
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

        // GET: api/categories/{categoryId}/results  (Public)
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

        // ============================================================
        // PARTICIPANT VIEW: My own enrolments and results
        // GET: api/participants/{participantId}/results
        // ============================================================
        [HttpGet("participants/{participantId}/results")]
        public async Task<ActionResult<IEnumerable<Result>>> GetParticipantResults(int participantId)
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            // Participant can only view their own; Organiser/Admin can view anyone's
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

        // GET: api/results/{id}  (Public)
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

        // PUT: api/results/{id}  (Organiser Only)
        [HttpPut("results/{id}")]
        public async Task<IActionResult> UpdateResult(int id, Result result)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update results.");

            if (id != result.ResultID) return BadRequest("Invalid result data.");

            var existing = await _context.Results.FindAsync(id);
            if (existing == null) return NotFound("Result does not exist.");

            existing.FinishTime = result.FinishTime;
            existing.Position = result.Position;
            existing.Status = result.Status;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/results/{id}  (Organiser Only)
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