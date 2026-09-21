using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class Draw : BaseEntity
{
    public long LotteryId { get; set; }
    public Lottery Lottery { get; set; } = null!;
    
    public DateTime ScheduledStartTime { get; set; }
    public DrawStatus Status { get; set; }
    
    public ICollection<WinningNumber> WinningNumbers { get; set; } = new List<WinningNumber>();
    
    public decimal TotalSalesAmount { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}