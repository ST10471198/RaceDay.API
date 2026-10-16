using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using RaceDayAPI.Models;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }

        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Weather> Weathers { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organiser>().HasIndex(o => o.Email).IsUnique();
            modelBuilder.Entity<Participant>().HasIndex(p => p.Email).IsUnique();

            modelBuilder.Entity<Result>()
                .HasIndex(r => new { r.EventID, r.ParticipantID })
                .IsUnique();

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Results)
                .HasForeignKey(r => r.CategoryID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}