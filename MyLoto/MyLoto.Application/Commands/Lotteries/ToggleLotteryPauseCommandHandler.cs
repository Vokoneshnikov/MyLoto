using MediatR;
using Hangfire;
using MyLoto.Application.Common;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common.BackgroundJobs; // Подключаем твой менеджер джобов

namespace MyLoto.Application.Commands.Lotteries;

public class ToggleLotteryPauseCommandHandler : IRequestHandler<ToggleLotteryPauseCommand, Result<bool>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public ToggleLotteryPauseCommandHandler(
        ILotteryRepository lotteryRepository,
        IBackgroundJobClient backgroundJobClient)
    {
        _lotteryRepository = lotteryRepository;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Result<bool>> Handle(ToggleLotteryPauseCommand request, CancellationToken ct)
    {
        var lottery = await _lotteryRepository.GetByIdAsync(request.LotteryId, ct);
        
        if (lottery == null)
        {
            return Result<bool>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));
        }

        lottery.IsPaused = !lottery.IsPaused;
        
        await _lotteryRepository.UpdateAsync(lottery, ct);

        // Перезапускаем цикл генерации тиражей, если лотерею сняли с паузы
        if (!lottery.IsPaused)
        {
            _backgroundJobClient.Enqueue<DrawJobsManager>(x => x.TriggerCreateDraw(lottery.Id));
        }

        return Result<bool>.Success(lottery.IsPaused);
    }
}