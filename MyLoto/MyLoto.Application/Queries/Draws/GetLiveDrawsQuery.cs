using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public record GetLiveDrawsQuery : IRequest<Result<IReadOnlyList<DrawDto>>>;