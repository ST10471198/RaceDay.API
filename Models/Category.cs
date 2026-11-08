using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    //Represents a race category
    //Category class
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Added default to prevent warnings

        [Required]
        [Column(TypeName = "decimal(6,2)")] // FIXED: Added precision mapping
        public decimal DistanceKM { get; set; } // FIXED: Changed double to decimal

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public int AgeMin { get; set; }

        [Required]
        public int AgeMax { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")] // Added precision mapping
        public decimal EntryFee { get; set; }

        // Foreign key to Event
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event? Event { get; set; } // Made nullable to prevent warnings

        // Navigation property
        public ICollection<Result> Results { get; set; } = new List<Result>(); // Initialized to prevent warnings
    }
}