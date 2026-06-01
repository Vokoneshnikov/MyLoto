using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id); 
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Login).IsUnique();

        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
        
        builder.Property(u => u.Balance).HasPrecision(18, 2); 
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}