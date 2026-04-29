using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShortLink.Domain.Entities;
using ShortLink.Infrastructure.Data.Identity;

namespace ShortLink.Infrastructure.Data.Configurations;

public class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.OriginalLink)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(_ => _.ShortCode)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(_ => _.ShortCode);

        builder.HasOne<ApplicationUser>()
                  .WithMany(_ => _.Urls)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
    }
}
