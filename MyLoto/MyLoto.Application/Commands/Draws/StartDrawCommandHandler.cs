using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class StartDrawCommandHandler : IRequestHandler<StartDrawCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartDrawCommandHandler(IDrawRepository drawRepository, ILotteryRepository lotteryRepository, IUnitOfWork unitOfWork)
    {
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(StartDrawCommand request, CancellationToken ct)
    {
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));
    
        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        if (lottery == null) return Result<Unit>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        if (!lottery.N.HasValue || !lottery.K.HasValue)
            return Result<Unit>.Failure(new Error("Lottery.InvalidConfig", "Параметры N или K не заданы"));

        draw.Status = DrawStatus.InProgress;

        // Теперь типы совпадают: WinningNumber
        draw.WinningNumbers = GenerateWinningNumbers(draw.Id, lottery.N.Value, lottery.K.Value);

        await _unitOfWork.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }

    private List<WinningNumber> GenerateWinningNumbers(long drawId, int n, int k)
    {
        var random = new Random();
        return Enumerable.Range(1, n)
            .OrderBy(_ => random.Next())
            .Take(k)
            .Select((num, index) => new WinningNumber 
            { 
                DrawId = drawId,
                Number = num, 
                Order = index + 1 // Было Position, стало Order
            })
            .ToList();
    }
}