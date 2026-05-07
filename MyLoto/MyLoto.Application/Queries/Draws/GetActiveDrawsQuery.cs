using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public record GetActiveDrawsQuery : IRequest<Result<IReadOnlyList<DrawDto>>>;