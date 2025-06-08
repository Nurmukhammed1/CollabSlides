using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CollabSlides.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PresentationUser many-to-many configuration
        modelBuilder.Entity<PresentationUser>()
            .HasKey(pu => new { pu.PresentationId, pu.UserId });

        modelBuilder.Entity<PresentationUser>()
            .HasOne(pu => pu.Presentation)
            .WithMany(p => p.PresentationUsers)
            .HasForeignKey(pu => pu.PresentationId);

        modelBuilder.Entity<PresentationUser>()
            .HasOne(pu => pu.User)
            .WithMany(u => u.PresentationUsers)
            .HasForeignKey(pu => pu.UserId);

        // Presentation configuration
        modelBuilder.Entity<Presentation>()
            .HasOne(p => p.Creator)
            .WithMany(u => u.CreatedPresentations)
            .HasForeignKey(p => p.CreatorId);

        // Slide configuration
        modelBuilder.Entity<Slide>()
            .HasOne(s => s.Presentation)
            .WithMany(p => p.Slides)
            .HasForeignKey(s => s.PresentationId);

        // Store TextBlock list as JSON
        modelBuilder.Entity<Slide>()
            .Property(s => s.Content)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<TextBlock>>(v, (JsonSerializerOptions?)null) ?? new List<TextBlock>());
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Presentation> Presentations { get; set; }
    public DbSet<PresentationUser> PresentationUsers { get; set; }
    public DbSet<Slide> Slides { get; set; }
}