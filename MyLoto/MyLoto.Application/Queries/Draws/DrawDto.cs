namespace MyLoto.Application.Queries.Draws;

public record DrawDto
{
    public long Id { get; init; }
    public long LotteryId { get; init; }
    public string LotteryName { get; init; } = string.Empty;
    public int DrawNumber { get; init; }
    public decimal TicketPrice { get; init; }
    public decimal Jackpot { get; init; }
    public DateTime SalesEndTime { get; init; }
}