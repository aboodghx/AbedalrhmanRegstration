using Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Name)
            .HasMaxLength(100);

        builder
            .HasIndex(x => x.Name)
            .IsUnique();

        builder
            .HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter("IsVerifyEmail = 0");

        builder
            .HasIndex(x => x.ICNumber)
            .IsUnique();

        builder
            .HasIndex(x => x.PhoneNumber)
            .IsUnique()
            .HasFilter("IsVerifyPhoneNumber = 0");

        builder
            .Property(x => x.PhoneNumber)
            .HasMaxLength(14);
    }
}