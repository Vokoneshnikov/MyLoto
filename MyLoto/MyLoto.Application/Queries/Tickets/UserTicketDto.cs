namespace MyLoto.Application.Queries.Tickets;

public class UserTicketDto
{
    public long TicketId { get; init; }
    public long DrawId { get; init; }
    public string LotteryName { get; init; } = string.Empty;
    public List<int> ChosenNumbers { get; init; } = [];
    public bool IsChecked { get; init; }
    public decimal WinAmount { get; init; }
    public DateTime PurchasedAt { get; init; }
}