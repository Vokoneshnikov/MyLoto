namespace MyLoto.Application.Queries.Lotteries;

public record LotteryDto(
    long Id,
    string Name,
    string Description,
    decimal TicketPrice,
    string Type,
    int? K,
    int? N);