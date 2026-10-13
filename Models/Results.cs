using RaceDayAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        // Foreign keys
        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public Event Event { get; set; }

        public int ParticipantID { get; set; }
        [ForeignKey("ParticipantID")]
        public Participant Participant { get; set; }

        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        // Race data
        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Registered";
    }
}
