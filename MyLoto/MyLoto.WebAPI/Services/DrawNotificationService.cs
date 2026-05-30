using Microsoft.AspNetCore.SignalR;
using MyLoto.Application.Abstractions;
using MyLoto.WebAPI.Hubs;

namespace MyLoto.WebAPI.Services;

public class DrawNotificationService : IDrawNotificationService
{
    private readonly IHubContext<DrawHub> _hubContext;

    public DrawNotificationService(IHubContext<DrawHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNumberAsync(long drawId, int number, int order, CancellationToken ct)
    {
        await _hubContext.Clients.Group($"draw_{drawId}").SendAsync("ReceiveNumber", new 
        { 
            Number = number, 
            Order = order 
        }, ct);
    }

    public async Task SendDrawFinishedAsync(long drawId, CancellationToken ct)
    {
        await _hubContext.Clients.Group($"draw_{drawId}").SendAsync("DrawFinished", ct);
    }
}