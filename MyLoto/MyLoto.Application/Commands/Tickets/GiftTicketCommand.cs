using MediatR;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Commands.Tickets;

public record GiftTicketCommand(
    long TicketId, 
    string RecipientLogin) : IRequest<Result<bool>>;