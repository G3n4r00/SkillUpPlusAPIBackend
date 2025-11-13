using Microsoft.EntityFrameworkCore;
using SkillUpPlus.Models;


namespace SkillUpPlus.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<InterestTag> InterestTags { get; set; }
        public DbSet<UserInterest> UserInterests { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para garantir que usuário não conclua o mesmo módulo duas vezes
            modelBuilder.Entity<UserProgress>()
                .HasIndex(up => new { up.UserId, up.ModuleId })
                .IsUnique();

            // Configuração da Chave Composta para UserInterest (Muitos-para-Muitos)
            modelBuilder.Entity<UserInterest>()
                .HasKey(ui => new { ui.UserId, ui.InterestTagId });

            modelBuilder.Entity<UserInterest>()
                .HasOne(ui => ui.User)
                .WithMany(u => u.Interests)
                .HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<UserInterest>()
                .HasOne(ui => ui.InterestTag)
                .WithMany(t => t.UserInterests)
                .HasForeignKey(ui => ui.InterestTagId);
        }
    }
}
