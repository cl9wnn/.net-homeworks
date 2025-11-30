using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(128);
        
        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Property(u => u.UpdatedDate)
            .IsRequired();
        
        builder.HasIndex(u => u.Username)
            .IsUnique();
    }
}