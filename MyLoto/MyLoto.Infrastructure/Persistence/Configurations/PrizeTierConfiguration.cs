using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class PrizeTierConfiguration : IEntityTypeConfiguration<PrizeTier>
{
    public void Configure(EntityTypeBuilder<PrizeTier> builder)
    {
        builder.Property(p => p.RewardValue).HasPrecision(18, 2);
        builder.HasOne(p => p.Lottery)
            .WithMany(l => l.PrizeTiers)
            .HasForeignKey(p => p.LotteryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}