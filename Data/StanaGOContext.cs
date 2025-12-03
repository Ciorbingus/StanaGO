using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;
using StanaGO.Models;
namespace StanaGO.Data
{
    public class StanaGOContext : IdentityDbContext<User>   // Legarea logicii cu baza de date
    {
        public StanaGOContext ( DbContextOptions<StanaGOContext> options ) : base (options) { }


        public DbSet<Casual> Casuals { get; set; }
        public DbSet<Shepherd> Shepherds { get; set; }
        public DbSet<Moderator> Moderators { get; set; }

        public DbSet<Sheepfarm> Sheepfarms { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Threat> Threats { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating ( ModelBuilder builder )
        {
            base.OnModelCreating (builder);

            builder.Entity<PrivateMessage> ()
                .HasOne (m => m.Sender)
                .WithMany () 
                .HasForeignKey (m => m.SenderId)
                .OnDelete (DeleteBehavior.ClientSetNull); 

            builder.Entity<PrivateMessage> ()
                .HasOne (m => m.Receiver)
                .WithMany ()
                .HasForeignKey (m => m.ReceiverId)
                .OnDelete (DeleteBehavior.ClientSetNull);

            builder.Entity<Report> ()
                .HasOne (r => r.Moderator)
                .WithMany (m => m.ResolvedReports)
                .HasForeignKey (r => r.ModeratorId)
                .OnDelete (DeleteBehavior.ClientSetNull);

            builder.Entity<Profile>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                      .HasMaxLength(450);

                entity.HasOne(p => p.User)
                      .WithOne()
                      .HasForeignKey<Profile>(p => p.Id)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}