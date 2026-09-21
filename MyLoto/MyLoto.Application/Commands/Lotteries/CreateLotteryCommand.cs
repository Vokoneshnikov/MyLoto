using MediatR;
using MyLoto.Application.Common;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Lotteries
{
    public record CreateLotteryCommand(
        string Name,
        string Description,
        decimal TicketPrice,
        LotteryType Type,
        decimal JackpotValue,
        TimeSpan TicketSalesDuration,
        TimeSpan DrawProcessingDuration,
        List<PrizeTierDto> PrizeTiers,
        int? K = null,
        int? N = null,
        int? Rows = null,
        int? Columns = null,
        int? MaxBallValue = null,
        int? JackpotThreshold = null,
        bool IsPaused = false
    ) : IRequest<Result<long>>;
}