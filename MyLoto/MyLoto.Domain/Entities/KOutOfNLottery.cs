using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class KOutOfNLottery : Lottery
{
    public KOutOfNLottery()
    {
        Type = LotteryType.K_Out_Of_N;
    }

    /// <summary>
    /// Сколько чисел выбирает пользователь.
    /// Например, 6 в лотерее "6 из 45".
    /// </summary>
    public int NumbersToChoose { get; set; }

    /// <summary>
    /// Максимальное число в диапазоне.
    /// Например, 45 в лотерее "6 из 45".
    /// </summary>
    public int MaxNumber { get; set; }
}