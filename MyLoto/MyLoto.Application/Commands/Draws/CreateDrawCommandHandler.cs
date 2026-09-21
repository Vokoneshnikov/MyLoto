using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using Hangfire;
using MyLoto.Application.Abstractions.Common.BackgroundJobs;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class CreateDrawCommandHandler : IRequestHandler<CreateDrawCommand, Result<long>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IRepository<Draw> _drawRepository;
    private readonly IUnitOfWork _unitOfWork;

    // ЧИСТОТА: Убрали валидатор из конструктора
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
        // 1. Проверяем, существует ли лотерея (бизнес-логика, оставляем здесь)
        var lottery = await _lotteryRepository.GetByIdAsync(request.LotteryId, ct);
        if (lottery == null)
            return Result<long>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        if (lottery.IsPaused)
            return Result<long>.Failure(new Error("Lottery.Paused", "Создание тиражей приостановлено"));
        
        // 2. Создаем сущность Draw
        var scheduledStartTime = DateTime.UtcNow.Add(lottery.TicketSalesDuration);
        
        var draw = new Draw
        {
            LotteryId = lottery.Id,
            ScheduledStartTime = scheduledStartTime,
            Status = DrawStatus.Pending,
            TotalSalesAmount = 0,
            WinningNumbers = new List<WinningNumber>(),
            Tickets = new List<Ticket>()
        };

        // 3. Сохранение в репозиторий
        await _drawRepository.AddAsync(draw, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        // 4. Планируем запуск тиража в Hangfire
        BackgroundJob.Schedule<DrawJobsManager>(
            x => x.TriggerStartDraw(draw.Id), 
            scheduledStartTime);

        return Result<long>.Success(draw.Id);
    }
}