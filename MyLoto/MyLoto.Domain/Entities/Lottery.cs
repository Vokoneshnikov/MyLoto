using MyLoto.Domain.Common;
using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class Lottery : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal TicketPrice { get; set; }
    public LotteryType Type { get; set; } // K_Out_Of_N, Bingo
    
    public int? K { get; set; }
    public int? N { get; set; }
    
    public double? PrizePoolPercentage { get; set; } // Например, 0.5 (50% от продаж идет в фонд)
    public decimal? AccumulatedJackpot { get; set; }
    
    public bool IsPaused { get; set; }
    
    public ICollection<Draw> Draws { get; set; } = new List<Draw>();
    public ICollection<PrizeTier> PrizeTiers { get; set; } = new List<PrizeTier>();
}