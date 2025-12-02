using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class UserRegistrationConfiguration: IEntityTypeConfiguration<UserRegistration>
{
    public void Configure(EntityTypeBuilder<UserRegistration> builder)
    {
        builder.ToTable("UserRegistrations");
        
        builder.HasKey(u => u.UserId);   

        builder.Property(u => u.UserId)
            .IsRequired();
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(u => u.RegisteredAt)
            .IsRequired();
        
        builder.HasIndex(u => u.Username)
            .IsUnique();
    }
}