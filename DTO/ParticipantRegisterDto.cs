using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTO
{
    public class ParticipantRegisterDto
    {
        [Required] public string Name { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string IdNumber { get; set; }
        [Required] public string EmergencyContact { get; set; }
    }
}