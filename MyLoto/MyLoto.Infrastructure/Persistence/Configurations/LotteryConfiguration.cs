using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class LotteryConfiguration : IEntityTypeConfiguration<Lottery>
{
    public void Configure(EntityTypeBuilder<Lottery> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(l => l.TicketPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(l => l.AccumulatedJackpot)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(l => l.IsPaused)
            .IsRequired();

        builder.Property(l => l.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder
            .HasDiscriminator(l => l.Type)
            .HasValue<KOutOfNLottery>(LotteryType.K_Out_Of_N)
            .HasValue<BingoLottery>(LotteryType.Bingo);

        builder.HasMany(l => l.Draws)
            .WithOne(d => d.Lottery)
            .HasForeignKey(d => d.LotteryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.PrizeTiers)
            .WithOne(p => p.Lottery)
            .HasForeignKey(p => p.LotteryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}