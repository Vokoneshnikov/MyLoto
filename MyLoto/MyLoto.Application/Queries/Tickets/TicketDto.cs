namespace MyLoto.Application.Queries.Tickets;

public class TicketDto
{
    public long Id { get; init; }
    public long LotteryId { get; init; }
    public List<int> Numbers { get; init; } = new();
    public bool IsChecked { get; init; }
    public bool IsWinning { get; init; }
    public decimal WinAmount { get; init; }
    public DateTime CreatedAt { get; init; }
}