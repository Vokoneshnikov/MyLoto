using MediatR;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Abstractions.Common.BackgroundJobs;

public class DrawJobsManager
{
    private readonly ISender _sender;

    public DrawJobsManager(ISender sender)
    {
        _sender = sender;
    }
    
    public async Task TriggerStartDraw(long drawId)
    {
        await _sender.Send(new StartDrawCommand(drawId));
    }
    
    public async Task TriggerCreateDraw(long lotteryId)
    {
        await _sender.Send(new CreateDrawCommand(lotteryId));
    }
}