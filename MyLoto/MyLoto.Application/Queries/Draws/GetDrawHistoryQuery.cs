using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public record GetDrawHistoryQuery(long LotteryId) : IRequest<Result<IReadOnlyList<DrawHistoryDto>>>;