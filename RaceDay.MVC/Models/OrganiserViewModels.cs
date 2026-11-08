using Microsoft.AspNetCore.Http;

namespace RaceDay.MVC.Models
{
    public class EventViewModel
    {
        public int EventID { get; set; }
        public int OrganiserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = new();
    }

    public class CreateEventModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now.AddDays(7);
        public string Location { get; set; } = string.Empty;
        public int MaxParticipants { get; set; } = 100;
        public string Status { get; set; } = "Open";
        public IFormFile? BannerImage { get; set; }
    }

    public class CategoryViewModel
    {
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DistanceKM { get; set; }
        public decimal EntryFee { get; set; }
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public TimeSpan StartTime { get; set; }
    }

    public class EnrolmentViewModel
    {
        public int ResultID { get; set; }
        public int EventID { get; set; }
        public int ParticipantID { get; set; }
        public int CategoryID { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
    }
}