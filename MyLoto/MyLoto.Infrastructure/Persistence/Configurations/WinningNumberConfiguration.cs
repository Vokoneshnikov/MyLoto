using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class WinningNumberConfiguration : IEntityTypeConfiguration<WinningNumber>
{
    public void Configure(EntityTypeBuilder<WinningNumber> builder)
    {
        builder.ToTable("DrawWinningNumbers"); 

        builder.HasKey(wn => new { wn.DrawId, wn.Order });

        builder.Property(wn => wn.DrawId)
            .HasColumnName("DrawId");

        builder.Property(wn => wn.Number)
            .HasColumnName("Number");

        builder.Property(wn => wn.Order)
            .HasColumnName("Order");

        builder.HasOne(wn => wn.Draw)
            .WithMany(d => d.WinningNumbers)
            .HasForeignKey(wn => wn.DrawId);
    }
}