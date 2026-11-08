namespace RaceDay.API.DTO
{
    public class EnrolmentDto
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

    public class UpdateResultDto
    {
        public int ResultID { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
    }

    public class CreateEnrolmentDto
    {
        public int EventID { get; set; }
        public int ParticipantID { get; set; }
        public int CategoryID { get; set; }
    }
}