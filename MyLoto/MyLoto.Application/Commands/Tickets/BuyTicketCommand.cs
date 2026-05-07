using MediatR;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Commands.Tickets;

public record BuyTicketCommand(
    long UserId, 
    long DrawId,
    List<int> ChosenNumbers) : IRequest<Result<TicketDto>>;