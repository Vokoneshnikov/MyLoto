using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories; // Проверь название папки Abstractions
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class CreateDrawCommandHandler : IRequestHandler<CreateDrawCommand, Result<long>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IRepository<Draw> _drawRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDrawCommandHandler(
        ILotteryRepository lotteryRepository, 
        IRepository<Draw> drawRepository, 
        IUnitOfWork unitOfWork)
    {
        _lotteryRepository = lotteryRepository;
        _drawRepository = drawRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(CreateDrawCommand request, CancellationToken ct)
    {
        // 1. Проверяем, существует ли лотерея
        var lottery = await _lotteryRepository.GetByIdAsync(request.LotteryId, ct);
        if (lottery == null) 
            return Result<long>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        // 2. Создаем сущность Draw согласно твоей модели
        var draw = new Draw
        {
            LotteryId = lottery.Id,
            ScheduledStartTime = request.EndDate, // Используем твой ScheduledStartTime
            Status = DrawStatus.Pending,
            TotalSalesAmount = 0,
            WinningNumbers = new List<WinningNumber>(),
            Tickets = new List<Ticket>()
        };

        // 3. Используем AddAsync с правильным токеном ct
        await _drawRepository.AddAsync(draw, ct);
        
        // 4. Сохраняем изменения
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<long>.Success(draw.Id);
    }
}