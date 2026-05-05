using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class AdminLogConfiguration : IEntityTypeConfiguration<AdminLog>
{
    public void Configure(EntityTypeBuilder<AdminLog> builder)
    {
        builder.Property(a => a.Action).HasConversion<string>();
        builder.Property(a => a.Details).HasColumnType("jsonb");
    }
}