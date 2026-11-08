using Microsoft.AspNetCore.Http;

namespace RaceDay.MVC.Models
{
    public class ParticipantProfileViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
} 