using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CrmSaturdayOsloWeb.Models
{
    public partial class crmsatoslo_dbContext : DbContext
    {
        public virtual DbSet<Assessments> Assessments { get; set; }
        public virtual DbSet<SessionSpeakers> SessionSpeakers { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<Speakers> Speakers { get; set; }

        public crmsatoslo_dbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assessments>(entity =>
            {
                entity.HasKey(e => e.AssessmentId)
                    .HasName("PK__Assessme__3D2BF81E2EE14EC5");

                entity.Property(e => e.Attendee)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Assessments)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK__Assessmen__Sessi__73BA3083");
            });

            modelBuilder.Entity<SessionSpeakers>(entity =>
            {
                entity.HasKey(e => new { e.SpeakerId, e.SessionId })
                    .HasName("PK_SessionSpeaker");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.SessionSpeakers)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SessionSpeaker_SessionId");

                entity.HasOne(d => d.Speaker)
                    .WithMany(p => p.SessionSpeakers)
                    .HasForeignKey(d => d.SpeakerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SessionSpeaker_SpeakerId");
            });

            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK_Post");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Schedule).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<Speakers>(entity =>
            {
                entity.HasKey(e => e.SpeakerId)
                    .HasName("PK_Speaker");

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.ProfilePictureExtension).HasMaxLength(5);

                entity.Property(e => e.TwitterHandle).HasMaxLength(100);
            });
        }
    }
}