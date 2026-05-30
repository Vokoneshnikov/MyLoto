using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class GetDrawLiveStatusQueryHandler : IRequestHandler<GetDrawLiveStatusQuery, Result<DrawLiveStatusResponse>>
{
    private readonly IDrawRepository _drawRepository;

    public GetDrawLiveStatusQueryHandler(IDrawRepository drawRepository)
    {
        _drawRepository = drawRepository;
    }

    public async Task<Result<DrawLiveStatusResponse>> Handle(GetDrawLiveStatusQuery request, CancellationToken ct)
    {
        var draw = await _drawRepository.GetDrawForBroadcastAsync(request.DrawId, ct);
        if (draw == null) 
            return Result<DrawLiveStatusResponse>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // 1. Если тираж в ожидании, чисел еще нет
        if (draw.Status == DrawStatus.Pending)
        {
            return Result<DrawLiveStatusResponse>.Success(
                new DrawLiveStatusResponse("Pending", new List<int>()));
        }

        // 2. Если тираж завершен, отдаем абсолютно все числа
        if (draw.Status == DrawStatus.Completed)
        {
            var allNumbers = draw.WinningNumbers
                .OrderBy(n => n.Order)
                .Select(n => n.Number)
                .ToList();
                
            return Result<DrawLiveStatusResponse>.Success(
                new DrawLiveStatusResponse("Completed", allNumbers));
        }

        // 3. Если тираж в процессе (InProgress) — считаем срез строго от фактического старта
        var startTime = draw.UpdatedAt ?? DateTime.UtcNow;
        var totalSecondsElapsed = (DateTime.UtcNow - startTime).TotalSeconds;
        
        // Сколько бочонков успело улететь в эфир (каждые 4 секунды)
        int ballsDroppedCount = Math.Max(0, (int)Math.Floor(totalSecondsElapsed / 4) + 1);

        var alreadyDrawnNumbers = draw.WinningNumbers
            .OrderBy(n => n.Order)
            .Take(ballsDroppedCount)
            .Select(n => n.Number)
            .ToList();

        return Result<DrawLiveStatusResponse>.Success(
            new DrawLiveStatusResponse("InProgress", alreadyDrawnNumbers));
    }
}