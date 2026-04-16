using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShortLink.Domain.Entities;

namespace ShortLink.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(_ => _.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(_ => _.Links)
            .WithOne(_ => _.User)
            .HasForeignKey(_ => _.UserId);
    }
}
