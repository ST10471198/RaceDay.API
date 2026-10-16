using RaceDayAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Weather
    {
        [Key]
        public int WeatherID { get; set; }

        public DateTime ForecastDate { get; set; }

        public double Temperature { get; set; }

        public string Condition { get; set; } = string.Empty;

        public double WindSpeed { get; set; }

        public double Humidity { get; set; }

        // Foreign key to Event
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event Event { get; set; }
    }
}