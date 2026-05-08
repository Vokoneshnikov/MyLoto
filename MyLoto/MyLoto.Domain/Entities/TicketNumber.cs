namespace MyLoto.Domain.Entities;

public class TicketNumber
{
    public long TicketId { get; set; }

    public int Position { get; set; }

    public int Number { get; set; }

    public int? Row { get; set; }

    public int? Column { get; set; }
}