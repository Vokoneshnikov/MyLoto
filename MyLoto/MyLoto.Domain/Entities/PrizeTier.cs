using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class PrizeTier : BaseEntity
{
    public long LotteryId { get; set; }

    public Lottery Lottery { get; set; } = null!;

    /// <summary>
    /// Тип правила выплаты.
    /// </summary>
    public PrizeTierRuleType RuleType { get; set; }

    /// <summary>
    /// Условие выплаты.
    /// Для KOutOfN — количество совпавших чисел.
    /// Для Bingo — номер шара, на котором билет был закрыт.
    /// Для Jackpot — можно хранить JackpotThreshold.
    /// </summary>
    public int ConditionValue { get; set; }

    /// <summary>
    /// Множитель выплаты.
    /// Например, 5 означает x5 от стоимости билета.
    /// Для джекпота можно оставить 0, потому что он берется из AccumulatedJackpot.
    /// </summary>
    public decimal RewardMultiplier { get; set; }
}