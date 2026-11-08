namespace RaceDay.API.DTO
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal DistanceKM { get; set; }
        public TimeSpan StartTime { get; set; }
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public decimal EntryFee { get; set; }
    }
}