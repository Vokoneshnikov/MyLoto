using MediatR;
using MyLoto.Application.Common;
using MyLoto.Application.Abstractions.Repositories; // Твой неймспейс репозиториев

namespace MyLoto.Application.Commands.Lotteries;

public class ToggleLotteryPauseCommandHandler : IRequestHandler<ToggleLotteryPauseCommand, Result<bool>>
{
    private readonly ILotteryRepository _lotteryRepository; // Используй имя своего репозитория лотерей

    public ToggleLotteryPauseCommandHandler(ILotteryRepository lotteryRepository)
    {
        _lotteryRepository = lotteryRepository;
    }

    public async Task<Result<bool>> Handle(ToggleLotteryPauseCommand request, CancellationToken ct)
    {
        var lottery = await _lotteryRepository.GetByIdAsync(request.LotteryId, ct);
    
        if (lottery == null)
        {
            return Result<bool>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));
        }

        lottery.IsPaused = !lottery.IsPaused;
    
        // ИСПОЛЬЗУЕМ НАШ НОВЫЙ АСИНХРОННЫЙ МЕТОД
        await _lotteryRepository.UpdateAsync(lottery, ct);

        return Result<bool>.Success(lottery.IsPaused);
    }
}