using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

/// <summary>
/// Запрос на получение билетов текущего авторизованного пользователя
/// </summary>
/// <param name="IsArchive">true — архивные (IsChecked = true), false — активные (IsChecked = false)</param>
/// <param name="IsWon">null — все, true — только выигравшие (WinAmount > 0), false — только проигравшие (WinAmount == 0)</param>
public record GetUserTicketsQuery(
    bool IsArchive, 
    bool? IsWon = null
) : IRequest<Result<IReadOnlyList<UserTicketDto>>>;