using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public WeatherController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("events/{eventId}/weather")]
        public async Task<ActionResult<IEnumerable<Weather>>> GetWeatherForEvent(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var weather = await _context.Weathers.Where(w => w.EventID == eventId).ToListAsync();
            if (weather.Count == 0) return NotFound("No weather information found.");
            return Ok(weather);
        }

        [HttpGet("weather/{id}")]
        public async Task<ActionResult<Weather>> GetWeather(int id)
        {
            var w = await _context.Weathers.FindAsync(id);
            if (w == null) return NotFound("Weather record does not exist.");
            return Ok(w);
        }

        [HttpPost("events/{eventId}/weather")]
        public async Task<ActionResult<Weather>> CreateWeather(
            int eventId,
            [FromQuery] DateTime forecastDate,
            [FromQuery] double temperature,
            [FromQuery] string condition,
            [FromQuery] double windSpeed,
            [FromQuery] double humidity)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can add weather records.");

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            var weather = new Weather
            {
                ForecastDate = forecastDate,
                Temperature = temperature,
                Condition = condition,
                WindSpeed = windSpeed,
                Humidity = humidity,
                EventID = eventId
            };

            _context.Weathers.Add(weather);
            await _context.SaveChangesAsync();
            return StatusCode(201, weather);
        }

        [HttpPut("weather/{id}")]
        public async Task<IActionResult> UpdateWeather(
            int id,
            [FromQuery] DateTime forecastDate,
            [FromQuery] double temperature,
            [FromQuery] string condition,
            [FromQuery] double windSpeed,
            [FromQuery] double humidity)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update weather.");

            var existing = await _context.Weathers.FindAsync(id);
            if (existing == null) return NotFound("Weather record does not exist.");

            var ev = await _context.Events.FindAsync(existing.EventID);
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            existing.ForecastDate = forecastDate;
            existing.Temperature = temperature;
            existing.Condition = condition;
            existing.WindSpeed = windSpeed;
            existing.Humidity = humidity;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("weather/{id}")]
        public async Task<IActionResult> DeleteWeather(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can delete weather records.");

            var w = await _context.Weathers.FindAsync(id);
            if (w == null) return NotFound("Weather record does not exist.");

            var ev = await _context.Events.FindAsync(w.EventID);
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            _context.Weathers.Remove(w);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}