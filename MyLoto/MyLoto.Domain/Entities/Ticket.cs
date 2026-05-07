namespace MyLoto.Domain.Entities;


public class Ticket : BaseEntity
{
    public long DrawId { get; set; }
    public Draw Draw { get; set; } = null!;
    
    public long OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    
    public long? GifterId { get; set; }
    public User? Gifter { get; set; }
    
    public ICollection<TicketNumber> SelectedNumbers { get; set; } = new List<TicketNumber>();
    
    public bool IsChecked { get; set; }
    public decimal WinAmount { get; set; }
    
    public long? TransactionId { get; set; }
}