using RaceDay.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(50)]
        public string IdNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EmergencyContact { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? ProfilePictureUrl { get; set; } 

        // Navigation property
        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}