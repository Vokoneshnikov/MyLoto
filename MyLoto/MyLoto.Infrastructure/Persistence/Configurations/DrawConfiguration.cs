using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

public class DrawConfiguration : IEntityTypeConfiguration<Draw>
{
    public void Configure(EntityTypeBuilder<Draw> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Status).HasConversion<string>();
        builder.Property(d => d.TotalSalesAmount).HasPrecision(18, 2);

        // 1НФ: Выигрышные числа в отдельной таблице
        builder.OwnsMany(d => d.WinningNumbers, wn => 
        {
            wn.ToTable("DrawWinningNumbers");
            wn.WithOwner().HasForeignKey("DrawId");
            wn.HasKey("DrawId", "Number"); // Составной ключ
        });

        builder.HasMany(d => d.Tickets)
            .WithOne(t => t.Draw)
            .HasForeignKey(t => t.DrawId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}