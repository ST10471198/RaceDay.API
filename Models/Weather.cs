using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Weather
    {
        [Key]
        public int WeatherID { get; set; }

        [Required]
        public DateTime ForecastDate { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        [StringLength(100)]
        public string Condition { get; set; }

        [Required]
        public double WindSpeed { get; set; }

        [Required]
        public double Humidity { get; set; }

        // Foreign key to Event
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event Event { get; set; }
    }
}