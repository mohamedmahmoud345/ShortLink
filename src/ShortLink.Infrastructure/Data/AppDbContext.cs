using Microsoft.EntityFrameworkCore;
using ShortLink.Domain.Entities;

namespace ShortLink.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<ShortUrl> ShortUrls { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ClickEvent> ClickEvents { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
