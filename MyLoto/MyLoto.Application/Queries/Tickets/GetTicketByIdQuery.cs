using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

// Запрашиваем одиночный DTO по идентификатору
public record GetTicketByIdQuery(long TicketId) : IRequest<Result<TicketDto>>;