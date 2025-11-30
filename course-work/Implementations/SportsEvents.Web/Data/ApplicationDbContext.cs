using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsEvents.Web.Models;

namespace SportsEvents.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SportEvent> SportEvents { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        public DbSet<EventImage> EventImages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Registration -> SportEvent (many-to-one)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.SportEvent)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.SportEventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Registration -> Participant (many-to-one)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Participant)
                .WithMany(p => p.Registrations)
                .HasForeignKey(r => r.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
