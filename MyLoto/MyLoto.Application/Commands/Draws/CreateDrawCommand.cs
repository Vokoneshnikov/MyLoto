using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Draws;

public record CreateDrawCommand(long LotteryId, DateTime EndDate) : IRequest<Result<long>>;