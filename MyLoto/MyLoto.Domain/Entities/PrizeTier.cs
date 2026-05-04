using MyLoto.Domain.Common;

namespace MyLoto.Domain.Entities;

public class PrizeTier : BaseEntity
{
    public long LotteryId { get; set; }
    public Lottery Lottery { get; set; } = null!;

    // Условие: для K из N — это кол-во совпавших чисел.
    // Для Bingo — это может быть индекс шара (ход), на котором закрыли всё.
    public int MatchingCondition { get; set; } 
    
    // Значение: для K из N — множитель (x5.0). 
    // Для Bingo — доля от призового фонда (например, 0.3 для 30%).
    public decimal RewardValue { get; set; } 
}