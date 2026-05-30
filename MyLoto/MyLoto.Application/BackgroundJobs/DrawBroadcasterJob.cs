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

    public DrawBroadcasterJob(
        IDrawRepository drawRepository,
        IDrawNotificationService notificationService,
        IMediator mediator)
    {
        _drawRepository = drawRepository;
        _notificationService = notificationService;
        _mediator = mediator;
    }

    public async Task BroadcastAsync(long drawId, CancellationToken ct) 
    {
        var draw = await _drawRepository.GetDrawForBroadcastAsync(drawId, ct);
        if (draw == null) return;

        var numbers = draw.WinningNumbers.OrderBy(n => n.Order).ToList();

        foreach (var number in numbers)
        {
            ct.ThrowIfCancellationRequested();

            Log.Information($"[LIVE] Отправляем шар №{number.Number} для тиража {drawId}"); // ДОБАВИТЬ ЛОГ

            // Рассылаем через абстракцию (SignalR под капотом)
            await _notificationService.SendNumberAsync(drawId, number.Number, number.Order, ct);

            // Ждем 4 секунды
            await Task.Delay(TimeSpan.FromSeconds(4), ct);
        }

        // Трансляция завершена
        await _notificationService.SendDrawFinishedAsync(drawId, ct);

        // ОТВЕТ НА ТВОЙ 3 ВОПРОС: Запускаем расчет выигрышей!
        await _mediator.Send(new CheckPrizesCommand(drawId), ct);
    }
}