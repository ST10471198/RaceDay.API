using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using RaceDayAPI.Data;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    //CRUD operations for weather forecasts
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public WeatherController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/5/weather  (Public)
        [HttpGet("events/{eventId}/weather")]
        public async Task<ActionResult<IEnumerable<Models.Weather>>> GetWeatherForEvent(int eventId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            var weather = await _context.Weathers
                .Where(w => w.EventID == eventId)
                .ToListAsync();

            if (weather.Count == 0) return NotFound("No weather information found.");
            return Ok(weather);
        }

        // GET: api/weather/5  (Public)
        [HttpGet("weather/{id}")]
        public async Task<ActionResult<Models.Weather>> GetWeather(int id)
        {
            var w = await _context.Weathers.FindAsync(id);
            if (w == null) return NotFound("Weather record does not exist.");
            return Ok(w);
        }

        // POST: api/events/5/weather  (Organiser Only)
        [HttpPost("events/{eventId}/weather")]
        public async Task<ActionResult<Models.Weather>> CreateWeather(int eventId, Models.Weather weather)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can add weather records.");

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Event does not exist.");

            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            if (weather == null) return BadRequest("Invalid weather information.");

            weather.EventID = eventId;
            _context.Weathers.Add(weather);
            await _context.SaveChangesAsync();

            return StatusCode(201, weather);
        }

        // PUT: api/weather/5  (Organiser Only)
        [HttpPut("weather/{id}")]
        public async Task<IActionResult> UpdateWeather(int id, Models.Weather weather)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Unauthorized("Only Organisers can update weather.");

            if (id != weather.WeatherID) return BadRequest("Invalid information.");

            var existing = await _context.Weathers.FindAsync(id);
            if (existing == null) return NotFound("Weather record does not exist.");

            var ev = await _context.Events.FindAsync(existing.EventID);
            if (ev.OrganiserID != HttpContext.Session.GetInt32("UserId"))
                return StatusCode(403, "You do not own this event.");

            existing.ForecastDate = weather.ForecastDate;
            existing.Temperature = weather.Temperature;
            existing.Condition = weather.Condition;
            existing.WindSpeed = weather.WindSpeed;
            existing.Humidity = weather.Humidity;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/weather/5  (Organiser Only)
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
