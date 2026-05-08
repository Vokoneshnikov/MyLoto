using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.WinAmount)
            .HasPrecision(18, 2);

        builder.OwnsMany(t => t.SelectedNumbers, sn =>
        {
            sn.ToTable("TicketNumbers");

            sn.WithOwner()
                .HasForeignKey("TicketId");

            sn.Property(x => x.Position)
                .IsRequired();

            sn.Property(x => x.Number)
                .IsRequired();

            sn.Property(x => x.Row);

            sn.Property(x => x.Column);

            sn.HasKey("TicketId", "Position");

            sn.HasIndex("TicketId", "Number")
                .IsUnique();
        });

        builder.HasOne(t => t.Owner)
            .WithMany(u => u.OwnedTickets)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Gifter)
            .WithMany()
            .HasForeignKey(t => t.GifterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}