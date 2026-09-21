namespace MyLoto.Application.Queries.Tickets;

public record UserTicketDto(
    long TicketId,
    long DrawId,
    List<int> SelectedNumbers,    // Числа, выбранные игроком
    bool IsChecked,
    decimal WinAmount,
    string DrawStatus,             // Статус тиража (Pending, InProgress, Completed)
    List<int> DrawWinningNumbers    // Выигрышные числа для красивой подсветки совпадений
);