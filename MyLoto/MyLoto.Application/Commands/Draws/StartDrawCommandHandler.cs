using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using FluentValidation;
using Hangfire;
using MyLoto.Application.BackgroundJobs;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class StartDrawCommandHandler : IRequestHandler<StartDrawCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<StartDrawCommand> _validator;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public StartDrawCommandHandler(
        IDrawRepository drawRepository, 
        ILotteryRepository lotteryRepository, 
        IUnitOfWork unitOfWork,
        IValidator<StartDrawCommand> validator,
        IBackgroundJobClient backgroundJobClient) 
    {
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _backgroundJobClient = backgroundJobClient; 
    }

    public async Task<Result<Unit>> Handle(StartDrawCommand request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<Unit>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        var draw = await _drawRepository.GetDrawForBroadcastAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // Если тираж уже идет, ничего не делаем (предохранитель)
        if (draw.Status == DrawStatus.InProgress)
        {
            return Result<Unit>.Success(Unit.Value);
        }

        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        if (lottery == null) return Result<Unit>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        if (draw.Status != DrawStatus.Pending) 
            return Result<Unit>.Failure(new Error("Draw.InvalidStatus", "Запустить можно только тираж в статусе ожидания"));
        
        draw.Status = DrawStatus.InProgress;

        // Генерация чисел (остается твоя отличная логика)
        if (lottery is KOutOfNLottery kLottery)
            draw.WinningNumbers = GenerateKOutOfNWinningNumbers(kLottery.NumbersToChoose, kLottery.MaxNumber);
        else if (lottery is BingoLottery bingoLottery)
            draw.WinningNumbers = GenerateBingoWinningNumbers(bingoLottery.MaxBallValue);
        else
            return Result<Unit>.Failure(new Error("Lottery.InvalidType", "Невалидный тип лотереи"));

        await _unitOfWork.SaveChangesAsync(ct);

        // ЗАПУСКАЕМ ТРАНСЛЯЦИЮ ЧЕРЕЗ HANGFIRE
        _backgroundJobClient.Enqueue<DrawBroadcasterJob>(x => x.BroadcastAsync(draw.Id, CancellationToken.None));

        return Result<Unit>.Success(Unit.Value);
    }

    private List<WinningNumber> GenerateKOutOfNWinningNumbers(int n, int k)
    {
        var random = new Random();
        return Enumerable.Range(1, n)
            .OrderBy(_ => random.Next())
            .Take(k)
            .Select((num, index) => new WinningNumber
            {
                Number = num,
                Order = index + 1
            })
            .ToList();
    }

    private List<WinningNumber> GenerateBingoWinningNumbers(int maxBallValue)
    {
        var random = new Random();
        return Enumerable.Range(1, maxBallValue)
            .OrderBy(_ => random.Next())
            .Take(90)
            .Select((num, index) => new WinningNumber
            {
                Number = num,
                Order = index + 1
            })
            .ToList();
    }
}