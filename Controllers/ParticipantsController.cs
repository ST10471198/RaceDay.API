using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTO;
using RaceDay.API.Services;
using RaceDayAPI.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly IBlobStorageService _blobService;

        public ParticipantsController(RaceDayDbContext context, IBlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParticipantResponseDto>> GetParticipant(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return Unauthorized();

            var participant = await _context.Participants.FindAsync(id);
            if (participant == null) return NotFound();

            return Ok(new ParticipantResponseDto
            {
                ParticipantID = participant.ParticipantID,
                Name = participant.Name,
                Email = participant.Email,
                EmergencyContact = participant.EmergencyContact,
                DateOfBirth = participant.DateOfBirth,
                ProfilePictureUrl = participant.ProfilePictureUrl
            });
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateParticipant(int id, [FromForm] UpdateParticipantDto dto)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return Unauthorized();

            var participant = await _context.Participants.FindAsync(id);
            if (participant == null) return NotFound();

            if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
            {
                participant.ProfilePictureUrl = await _blobService.UploadFileAsync(dto.ProfilePicture);
            }

            participant.Name = dto.Name;
            participant.EmergencyContact = dto.EmergencyContact;
            participant.DateOfBirth = dto.DateOfBirth;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Profile updated successfully" });
        }
    }
}