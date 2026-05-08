namespace MyLoto.Application.Queries.Lotteries;

public record LotteryDto
{
    public long Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal TicketPrice { get; init; }

    public string Type { get; init; } = string.Empty;

    public decimal AccumulatedJackpot { get; init; }

    public bool IsPaused { get; init; }

    public int? NumbersToChoose { get; init; }

    public int? MaxNumber { get; init; }

    public int? Rows { get; init; }

    public int? Columns { get; init; }

    public int? MaxBallValue { get; init; }

    public int? JackpotThreshold { get; init; }
}