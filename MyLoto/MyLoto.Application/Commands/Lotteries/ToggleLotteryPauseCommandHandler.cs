using MediatR;
using Hangfire;
using MyLoto.Application.Abstractions.Common.BackgroundJobs;
using MyLoto.Application.Common;
using MyLoto.Application.Abstractions.Repositories;

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
        // До репозитория дойдут только структурно корректные ID лотерей (> 0)
        var lottery = await _lotteryRepository.GetByIdAsync(request.LotteryId, ct);
        
        if (lottery == null)
        {
            return Result<bool>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));
        }

        // Инвертируем текущее состояние паузы
        lottery.IsPaused = !lottery.IsPaused;
        
        await _lotteryRepository.UpdateAsync(lottery, ct);

        // Бизнес-логика: если лотерею сняли с паузы, немедленно пинаем Hangfire-менеджер,
        // чтобы он сгенерировал новый активный тираж взамен пропущенных.
        if (!lottery.IsPaused)
        {
            _backgroundJobClient.Enqueue<DrawJobsManager>(x => x.TriggerCreateDraw(lottery.Id));
        }

        // Возвращаем новое состояние флага паузы (true/false)
        return Result<bool>.Success(lottery.IsPaused);
    }
}