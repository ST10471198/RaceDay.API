namespace RaceDay.API.DTO
{
    public class ApiRegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Participant";

        // Organiser specific field
        public string? Phone { get; set; }

        // Participant specific fields
        public string? DateOfBirth { get; set; }
        public string? IdNumber { get; set; }
        public string? EmergencyContact { get; set; }
    }

    public class ApiLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}