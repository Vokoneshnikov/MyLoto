using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using FluentValidation;
using Hangfire;
using MyLoto.Application.Common.BackgroundJobs;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class CreateDrawCommandHandler : IRequestHandler<CreateDrawCommand, Result<long>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IRepository<Draw> _drawRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateDrawCommand> _validator;

    public CreateDrawCommandHandler(
        ILotteryRepository lotteryRepository, 
        IRepository<Draw> drawRepository, 
        IUnitOfWork unitOfWork,
        IValidator<CreateDrawCommand> validator)
    {
        _lotteryRepository = lotteryRepository;
        _drawRepository = drawRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<long>> Handle(CreateDrawCommand request, CancellationToken ct)
    {
        // Валидируем команду
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<long>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        // 1. Проверяем, существует ли лотерея
        var lottery = await _lotteryRepository.GetByIdAsync(request.LotteryId, ct);
        if (lottery == null)
            return Result<long>.Failure(new Error("Lottery.NotFound", "Лотерея не найдена"));

        if (lottery.IsPaused)
            return Result<long>.Failure(new Error("Lottery.Paused", "Создание тиражей приостановлено"));
        
        // 2. Создаем сущность Draw согласно твоей модели
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

        // 3. Используем AddAsync с правильным токеном ct
        await _drawRepository.AddAsync(draw, ct);

        // 4. Сохраняем изменения
        await _unitOfWork.SaveChangesAsync(ct);
        
        // 5. Планируем запуск тиража в Hangfire
        BackgroundJob.Schedule<DrawJobsManager>(
            x => x.TriggerStartDraw(draw.Id), 
            scheduledStartTime);

        return Result<long>.Success(draw.Id);
    }
}