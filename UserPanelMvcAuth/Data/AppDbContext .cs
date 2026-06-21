using Microsoft.EntityFrameworkCore;
using UserPanelMvcAuth.Models;

namespace UserPanelMvcAuth.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<UserNote> UserNotes => Set<UserNote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.CreatedAt)
                    .IsRequired();

                entity.HasMany(u => u.Notes)
                    .WithOne(n => n.AppUser)
                    .HasForeignKey(n => n.AppUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserNote>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.Property(n => n.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(n => n.Content)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(n => n.CreatedAt)
                    .IsRequired();
            });
        }
    }
}
