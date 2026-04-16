using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShortLink.Domain.Entities;

namespace ShortLink.Infrastructure.Data.Configurations;

public class ClickEventconfiguration : IEntityTypeConfiguration<ClickEvent>
{
    public void Configure(EntityTypeBuilder<ClickEvent> builder)
    {
        builder.HasKey(_ => _.Id);

        builder.Property(x => x.DeviceType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
