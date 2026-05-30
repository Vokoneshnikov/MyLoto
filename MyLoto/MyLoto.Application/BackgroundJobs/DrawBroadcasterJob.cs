using Hangfire;
using MediatR;
using MyLoto.Application.Abstractions;
using Serilog;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.BackgroundJobs;

public class DrawBroadcasterJob
{
    private readonly IDrawRepository _drawRepository;
    private readonly IDrawNotificationService _notificationService;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public DrawBroadcasterJob(
        IDrawRepository drawRepository,
        IDrawNotificationService notificationService,
        IMediator mediator,
        IUnitOfWork unitOfWork) // Внедряем UnitOfWork через DI
    {
        _drawRepository = drawRepository;
        _notificationService = notificationService;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task BroadcastAsync(long drawId, CancellationToken ct) 
    {
        var draw = await _drawRepository.GetDrawForBroadcastAsync(drawId, ct);
        if (draw == null) return;

        var numbers = draw.WinningNumbers.OrderBy(n => n.Order).ToList();
        
        draw.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);

        foreach (var number in numbers)
        {
            ct.ThrowIfCancellationRequested();

            // Добавлен информативный лог с фиксацией порядка выпадения
            Log.Information($"[LIVE] Отправляем шар №{number.Number} для тиража {drawId} (Порядок/Order: {number.Order})");

            // Рассылаем через абстракцию (SignalR под капотом)
            await _notificationService.SendNumberAsync(drawId, number.Number, number.Order, ct);

            // Ждем ровно 4 секунды перед следующим шаром
            await Task.Delay(TimeSpan.FromSeconds(4), ct);
        }

        // Трансляция завершена
        await _notificationService.SendDrawFinishedAsync(drawId, ct);

        await _mediator.Send(new CheckPrizesCommand(drawId), ct);
    }
}