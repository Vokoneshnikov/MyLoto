using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

public record GetUserTicketsQuery() : IRequest<Result<IReadOnlyList<UserTicketDto>>>;