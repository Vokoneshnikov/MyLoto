using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public abstract class Lottery : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal TicketPrice { get; set; }

    public LotteryType Type { get; set; }

    public decimal AccumulatedJackpot { get; set; }

    public bool IsPaused { get; set; }

    public ICollection<Draw> Draws { get; set; } = new List<Draw>();

    public ICollection<PrizeTier> PrizeTiers { get; set; } = new List<PrizeTier>();
    
    public TimeSpan TicketSalesDuration { get; set; } 
    
    public TimeSpan DrawProcessingDuration { get; set; }
}