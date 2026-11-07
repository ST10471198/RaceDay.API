namespace RaceDay.API.DTO
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Participant";

        // Organiser fields
        public string? Phone { get; set; }

        // Participant fields
        public string? DateOfBirth { get; set; } // Using string for easy JSON transfer
        public string? IdNumber { get; set; }
        public string? EmergencyContact { get; set; }
    }
}