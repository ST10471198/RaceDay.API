public class RegisterDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // "Organiser" or "Participant"

    // Participant specific fields (will be null for Organisers)
    public DateTime? DateOfBirth { get; set; }
    public string? IdNumber { get; set; }
    public string? EmergencyContact { get; set; }

    // Organiser specific fields (will be null for Participants)
    public string? Phone { get; set; }
}
