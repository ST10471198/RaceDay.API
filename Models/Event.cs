using RaceDay.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    //Represents a race event created by an organiser
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        public int OrganiserID { get; set; }
        [ForeignKey("OrganiserID")]
        public Organiser? Organiser { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public int MaxParticipants { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Upcoming";

        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Weather> WeatherForecasts { get; set; } = new List<Weather>();
    }
}
