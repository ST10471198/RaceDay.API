using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTO;
using RaceDayAPI.Models;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("Events/{eventId}/enrolments")]
        public async Task<ActionResult<IEnumerable<EnrolmentDto>>> GetEnrolmentsForEvent(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can view enrolments.");

            // Join Result, Participant, and Category tables
            var enrolments = await _context.Results
                .Include(r => r.Participant)
                .Include(r => r.Category)
                .Where(r => r.EventID == eventId)
                .Select(r => new EnrolmentDto
                {
                    ResultID = r.ResultID,
                    EventID = r.EventID,
                    ParticipantID = r.ParticipantID,
                    CategoryID = r.CategoryID,
                    ParticipantName = r.Participant.Name,
                    CategoryName = r.Category.Name,
                    Status = r.Status,
                    FinishTime = r.FinishTime,
                    Position = r.Position
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        [HttpPost("Results/batch")]
        public async Task<IActionResult> SaveBatchResults([FromBody] List<UpdateResultDto> results)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can enter results.");

            foreach (var dto in results)
            {
                var result = await _context.Results.FindAsync(dto.ResultID);
                if (result != null)
                {
                    result.FinishTime = dto.FinishTime;
                    result.Position = dto.Position;
                    // Optional: If a finish time is entered, update status to 'Finished'
                    if (dto.FinishTime.HasValue) result.Status = "Finished";
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Results updated successfully" });
        }
    }
}