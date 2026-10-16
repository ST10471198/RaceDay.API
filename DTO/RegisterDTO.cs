using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Organiser" or "Participant"

        // Participant-specific (nullable)
        public DateTime? DateOfBirth { get; set; }
        public string? IdNumber { get; set; }
        public string? EmergencyContact { get; set; }

        // Organiser-specific (nullable)
        public string? Phone { get; set; }
    }
}