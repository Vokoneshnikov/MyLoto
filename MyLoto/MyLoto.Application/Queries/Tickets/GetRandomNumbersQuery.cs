using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

public record GetRandomNumbersQuery(long DrawId) : IRequest<Result<List<int>>>;