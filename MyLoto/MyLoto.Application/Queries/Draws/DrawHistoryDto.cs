namespace MyLoto.Application.Queries.Draws;

public record DrawHistoryDto
{
    public long Id { get; init; }
    public int DrawNumber { get; init; }
    public List<int> WinningNumbers { get; init; } = new();
    public decimal TotalPrizePool { get; init; }
    public int WinnersCount { get; init; }
    public DateTime DrawDate { get; init; }
}