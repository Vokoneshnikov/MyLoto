namespace MyLoto.Application.Queries.Draws;

public record DrawDto
{
    public long Id { get; init; }
    public long LotteryId { get; init; }
    public string LotteryName { get; init; } = string.Empty;
    public decimal TicketPrice { get; init; }
    public decimal Jackpot { get; init; }
    public DateTime SalesEndTime { get; init; }

    // --- Новые поля для правил ---
    public string LotteryType { get; init; } = string.Empty; // "Bingo" или "KOutOfN"
    public int? NumbersToChoose { get; init; }
    public int? MaxNumber { get; init; }
    public int? Rows { get; init; }
    public int? Columns { get; init; }
    public int? MaxBallValue { get; init; }
}