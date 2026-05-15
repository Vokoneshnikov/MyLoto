using Hangfire;
using MediatR;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Common.BackgroundJobs;

public class DrawJobsManager
{
    private readonly ISender _sender;

    public DrawJobsManager(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Вызывается Hangfire'ом, когда приходит время начать тираж.
    /// </summary>
    public async Task TriggerStartDraw(long drawId)
    {
        await _sender.Send(new StartDrawCommand(drawId));
    }

    /// <summary>
    /// Вызывается для автоматического создания следующего тиража.
    /// </summary>
    public async Task TriggerCreateDraw(long lotteryId)
    {
        await _sender.Send(new CreateDrawCommand(lotteryId));
    }
}