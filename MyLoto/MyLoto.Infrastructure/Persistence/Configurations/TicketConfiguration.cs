using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLoto.Domain.Entities;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.WinAmount).HasPrecision(18, 2);

        // 1НФ: Выбранные числа пользователя в отдельной таблице
        builder.OwnsMany(t => t.SelectedNumbers, sn => 
        {
            sn.ToTable("TicketNumbers");
            sn.WithOwner().HasForeignKey("TicketId");
            sn.HasKey("TicketId", "Number");
        });

        // Связь с владельцем
        builder.HasOne(t => t.Owner)
            .WithMany(u => u.OwnedTickets)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с дарителем (опционально)
        builder.HasOne(t => t.Gifter)
            .WithMany()
            .HasForeignKey(t => t.GifterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}