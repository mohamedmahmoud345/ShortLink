using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShortLink.Domain.Entities;

namespace ShortLink.Infrastructure.Data.Configurations;

public class ShortLinkConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.OriginalUrl)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(_ => _.ShortCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(_ => _.ShortCode);

        builder.HasMany(_ => _.ClickEvents)
            .WithOne(_ => _.ShortUrl)
            .HasForeignKey(_ => _.ShortLinkId);
    }
}
