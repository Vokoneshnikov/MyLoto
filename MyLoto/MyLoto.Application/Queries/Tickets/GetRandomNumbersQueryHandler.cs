using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Queries.Tickets;

public class GetRandomNumbersQueryHandler : IRequestHandler<GetRandomNumbersQuery, Result<List<int>>>
{
    private readonly IRepository<Draw> _drawRepository;
    private readonly ILotteryRepository _lotteryRepository;

    public GetRandomNumbersQueryHandler(
        IRepository<Draw> drawRepository, 
        ILotteryRepository lotteryRepository)
    {
        _drawRepository = drawRepository;
        _lotteryRepository = lotteryRepository;
    }

    public async Task<Result<List<int>>> Handle(GetRandomNumbersQuery request, CancellationToken ct)
    {
        // 1. Получаем тираж
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) 
            return Result<List<int>>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // 2. Получаем настройки лотереи для этого тиража
        var lottery = await _lotteryRepository.GetByIdAsync(draw.LotteryId, ct);
        if (lottery == null) 
            return Result<List<int>>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        // 3. Используем вынесенную логику генератора
        try
        {
            var numbers = LotteryGenerator.GenerateNumbers(lottery);
            return Result<List<int>>.Success(numbers);
        }
        catch (InvalidOperationException ex)
        {
            return Result<List<int>>.Failure(new Error("Lottery.TypeUnknown", ex.Message));
        }
    }
}