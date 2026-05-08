using MyLoto.Domain.Enums;

namespace MyLoto.Domain.Entities;

public class BingoLottery : Lottery
{
    public BingoLottery()
    {
        Type = LotteryType.Bingo;
    }

    /// <summary>
    /// Количество строк в билете.
    /// Например, 3.
    /// </summary>
    public int Rows { get; set; }

    /// <summary>
    /// Количество колонок в билете.
    /// Например, 10.
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// Максимальное значение шара.
    /// Например, 90.
    /// </summary>
    public int MaxBallValue { get; set; }

    /// <summary>
    /// Сколько первых выпавших шаров участвуют в проверке джекпота.
    /// Например, первые 5 шаров.
    /// </summary>
    public int JackpotThreshold { get; set; }

    /// <summary>
    /// Общее количество чисел в билете.
    /// Для Bingo считается как Rows * Columns.
    /// </summary>
    public int TicketNumbersCount => Rows * Columns;
}