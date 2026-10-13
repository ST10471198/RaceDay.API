using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public double DistanceKM { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public int AgeMin { get; set; }

        [Required]
        public int AgeMax { get; set; }

        [Required]
        public decimal EntryFee { get; set; }

        // Foreign key to Event
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event Event { get; set; }

        // Navigation property
        public ICollection<Result> Results { get; set; }
    }
}