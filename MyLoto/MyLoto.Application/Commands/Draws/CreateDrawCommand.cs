using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Draws;

public record CreateDrawCommand(long LotteryId) : IRequest<Result<long>>;