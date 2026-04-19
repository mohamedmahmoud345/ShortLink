using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShortLink.Domain.Entities;
using ShortLink.Infrastructure.Data.Identity;

namespace ShortLink.Infrastructure.Data;

public class AppDbContext :
    IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<ShortUrl> ShortUrls { get; set; }
    public DbSet<ClickEvent> ClickEvents { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
