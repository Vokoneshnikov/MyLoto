using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

public record GetUserTicketsQuery(long UserId) : IRequest<Result<IReadOnlyList<UserTicketDto>>>;