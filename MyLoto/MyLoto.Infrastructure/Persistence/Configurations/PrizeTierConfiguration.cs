using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class PrizeTierConfiguration : IEntityTypeConfiguration<PrizeTier>
{
    public void Configure(EntityTypeBuilder<PrizeTier> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.RuleType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.ConditionValue)
            .IsRequired();

        builder.Property(p => p.RewardMultiplier)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(p => p.Lottery)
            .WithMany(l => l.PrizeTiers)
            .HasForeignKey(p => p.LotteryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}