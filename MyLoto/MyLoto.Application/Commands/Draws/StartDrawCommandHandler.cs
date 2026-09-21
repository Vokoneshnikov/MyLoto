using MediatR;
using Hangfire;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.BackgroundJobs;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class StartDrawCommandHandler : IRequestHandler<StartDrawCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobClient _backgroundJobClient;

    // ЧИСТОТА: Валидатор успешно удален из конструктора
    public StartDrawCommandHandler(
        IDrawRepository drawRepository, 
        ILotteryRepository lotteryRepository, 
        IUnitOfWork unitOfWork,
        IBackgroundJobClient backgroundJobClient) 
    {
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
        _unitOfWork = unitOfWork;
        _backgroundJobClient = backgroundJobClient; 
    }

    public async Task<Result<Unit>> Handle(StartDrawCommand request, CancellationToken ct)
    {
        var draw = await _drawRepository.GetDrawForBroadcastAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // Предохранитель: если тираж уже запущен, просто выходим с успехом (идемпотентность)
        if (draw.Status == DrawStatus.InProgress)
        {
            return Result<Unit>.Success(Unit.Value);
        }

        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        if (lottery == null) return Result<Unit>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        if (draw.Status != DrawStatus.Pending) 
            return Result<Unit>.Failure(new Error("Draw.InvalidStatus", "Запустить можно только тираж в статусе ожидания"));
        
        draw.Status = DrawStatus.InProgress;

        // Генерация выигрышной комбинации
        if (lottery is KOutOfNLottery kLottery)
        {
            // Исправили порядок: генерируем из MaxNumber (N), выбираем NumbersToChoose (K)
            draw.WinningNumbers = GenerateKOutOfNWinningNumbers(kLottery.MaxNumber, kLottery.NumbersToChoose);
        }
        else if (lottery is BingoLottery bingoLottery)
        {
            draw.WinningNumbers = GenerateBingoWinningNumbers(bingoLottery.MaxBallValue);
        }
        else
        {
            return Result<Unit>.Failure(new Error("Lottery.InvalidType", "Невалидный тип лотереи"));
        }

        await _unitOfWork.SaveChangesAsync(ct);

        // Запуск трансляции (розыгрыша) в бэкграунде через Hangfire
        _backgroundJobClient.Enqueue<DrawBroadcasterJob>(x => x.BroadcastAsync(draw.Id, CancellationToken.None));

        return Result<Unit>.Success(Unit.Value);
    }

    private List<WinningNumber> GenerateKOutOfNWinningNumbers(int maxNumber, int numbersToChoose)
    {
        if (numbersToChoose > maxNumber)
        {
            throw new InvalidOperationException(
                $"Нельзя выбрать {numbersToChoose} выигрышных чисел из диапазона 1..{maxNumber}.");
        }

        var numbers = Enumerable.Range(1, maxNumber).ToList();
        Shuffle(numbers);

        return numbers
            .Take(numbersToChoose)
            .Select((num, index) => new WinningNumber
            {
                Number = num,
                Order = index + 1
            })
            .ToList();
    }

    private List<WinningNumber> GenerateBingoWinningNumbers(int maxBallValue)
    {
        var numbers = Enumerable.Range(1, maxBallValue).ToList();
        Shuffle(numbers);

        return numbers
            .Select((num, index) => new WinningNumber
            {
                Number = num,
                Order = index + 1
            })
            .ToList();
    }

    private static void Shuffle(List<int> numbers)
    {
        var random = Random.Shared;

        for (var i = numbers.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);

            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }
    }
}