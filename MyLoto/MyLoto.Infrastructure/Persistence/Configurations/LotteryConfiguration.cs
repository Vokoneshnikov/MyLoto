using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

public class LotteryConfiguration : IEntityTypeConfiguration<Lottery>
{
    public void Configure(EntityTypeBuilder<Lottery> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.TicketPrice).HasPrecision(18, 2);
        builder.Property(l => l.AccumulatedJackpot).HasPrecision(18, 2);
        
        // Храним Enum как строку
        builder.Property(l => l.Type).HasConversion<string>();

        // Связь с тиражами: при удалении лотереи тиражи удалять нельзя (Restrict)
        builder.HasMany(l => l.Draws)
            .WithOne(d => d.Lottery)
            .HasForeignKey(d => d.LotteryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}