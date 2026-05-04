namespace MyLoto.Domain.Enums;

public enum LotteryType
{
    /// <summary>
    /// Классическая числовая лотерея (например, 6 из 45).
    /// </summary>
    K_Out_Of_N = 1,

    /// <summary>
    /// Лотерея по типу бинго/столото (заполнение карточки).
    /// </summary>
    Bingo = 2
}