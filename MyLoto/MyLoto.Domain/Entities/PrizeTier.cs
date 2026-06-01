using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class PrizeTier : BaseEntity
{
    public long LotteryId { get; set; }

    public Lottery Lottery { get; set; } = null!;
    
    public PrizeTierRuleType RuleType { get; set; }
    
    public int ConditionValue { get; set; }
    
    public decimal RewardMultiplier { get; set; }
}