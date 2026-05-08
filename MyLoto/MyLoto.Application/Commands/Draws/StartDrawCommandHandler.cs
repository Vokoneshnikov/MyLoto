using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using FluentValidation;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class StartDrawCommandHandler : IRequestHandler<StartDrawCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<StartDrawCommand> _validator; // Добавляем валидатор

    public StartDrawCommandHandler(
        IDrawRepository drawRepository, 
        ILotteryRepository lotteryRepository, 
        IUnitOfWork unitOfWork,
        IValidator<StartDrawCommand> validator) // Внедряем через DI
    {
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Unit>> Handle(StartDrawCommand request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<Unit>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null)
            return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        if (lottery == null)
            return Result<Unit>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        if (draw.Status != DrawStatus.Pending) 
        {
            return Result<Unit>.Failure(new Error("Draw.InvalidStatus", "Запустить можно только тираж в статусе ожидания"));
        }
        
        draw.Status = DrawStatus.InProgress;
        
        Console.WriteLine($"Draw Status: {draw.Status}");

        // Генерация чисел для двух типов лотерей
        if (lottery is KOutOfNLottery kOutOfNLottery)
        {
            draw.WinningNumbers = GenerateKOutOfNWinningNumbers(kOutOfNLottery.NumbersToChoose, kOutOfNLottery.MaxNumber);
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
            .Take(90)  // 90 шаров для бинго
            .Select((num, index) => new WinningNumber
            {
                Number = num,
                Order = index + 1
            })
            .ToList();
    }
}