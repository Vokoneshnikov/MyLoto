using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Queries.Draws;

public class GetLiveDrawsQueryHandler : IRequestHandler<GetLiveDrawsQuery, Result<IReadOnlyList<DrawDto>>>
{
    private readonly IDrawRepository _drawRepository;

    public GetLiveDrawsQueryHandler(IDrawRepository drawRepository)
    {
        _drawRepository = drawRepository;
    }

    public async Task<Result<IReadOnlyList<DrawDto>>> Handle(
        GetLiveDrawsQuery request, 
        CancellationToken cancellationToken)
    {
        var liveDraws = await _drawRepository.GetLiveDrawsAsync(cancellationToken);

        var dtos = liveDraws.Select(d => new DrawDto
        {
            Id = d.Id,
            LotteryId = d.LotteryId,
            LotteryName = d.Lottery.Name,
            TicketPrice = d.Lottery.TicketPrice,
            Jackpot = d.Lottery.AccumulatedJackpot,
            SalesEndTime = d.ScheduledStartTime,
            LotteryType = d.Lottery.Type.ToString(),
            
            // Безопасно приводим к конкретным типам лотерей для заполнения специфичных полей DTO
            NumbersToChoose = d.Lottery is KOutOfNLottery kOfN ? kOfN.NumbersToChoose : null,
            MaxNumber = d.Lottery is KOutOfNLottery kOfN2 ? kOfN2.MaxNumber : null,
            
            Rows = d.Lottery is BingoLottery bingo ? bingo.Rows : null,
            Columns = d.Lottery is BingoLottery bingo2 ? bingo2.Columns : null,
            MaxBallValue = d.Lottery is BingoLottery bingo3 ? bingo3.MaxBallValue : null
        }).ToList();

        return Result<IReadOnlyList<DrawDto>>.Success(dtos);
    }
}