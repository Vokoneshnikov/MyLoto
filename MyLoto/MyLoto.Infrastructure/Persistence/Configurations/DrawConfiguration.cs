using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class DrawConfiguration : IEntityTypeConfiguration<Draw>
{
    public void Configure(EntityTypeBuilder<Draw> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Status).HasConversion<string>();
        builder.Property(d => d.TotalSalesAmount).HasPrecision(18, 2);

        builder.HasMany(d => d.WinningNumbers)
            .WithOne(wn => wn.Draw)
            .HasForeignKey(wn => wn.DrawId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Tickets)
            .WithOne(t => t.Draw)
            .HasForeignKey(t => t.DrawId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}