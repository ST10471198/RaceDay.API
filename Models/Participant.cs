using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    //Represents a participant (runner/walker/cyclist)
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(50)]
        public string IdNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string EmergencyContact { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property — one Participant has many Results
        public ICollection<Result> Results { get; set; }
    }
}