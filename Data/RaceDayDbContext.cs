using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using RaceDayAPI.Models;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options)
        {
        }

        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Weather> Weathers { get; set; }
        public DbSet<Result> Results { get; set; } // Changed to 'Result'

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Force EF Core to use the singular table names from your SQL script
            modelBuilder.Entity<Organiser>().ToTable("Organiser");
            modelBuilder.Entity<Participant>().ToTable("Participant");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Weather>().ToTable("Weather");
            modelBuilder.Entity<Result>().ToTable("Result"); // Changed to 'Result'
        }
    }
}