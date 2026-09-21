using MediatR;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Lotteries;

public record ToggleLotteryPauseCommand(int LotteryId) : IRequest<Result<bool>>;