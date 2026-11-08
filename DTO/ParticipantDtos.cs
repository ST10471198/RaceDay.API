namespace RaceDay.API.DTO
{
    public class UpdateParticipantDto
    {
        public string Name { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }

    public class ParticipantResponseDto
    {
        public int ParticipantID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}