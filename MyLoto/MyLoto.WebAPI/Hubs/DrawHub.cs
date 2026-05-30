using Microsoft.AspNetCore.SignalR;

namespace MyLoto.WebAPI.Hubs; // Подправь namespace под свой проект

public class DrawHub : Hub
{
    // Фронтенд будет вызывать этот метод при загрузке страницы тиража
    public async Task JoinDrawGroup(long drawId)
    {
        var groupName = $"draw_{drawId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    // Фронтенд вызывает при уходе со страницы
    public async Task LeaveDrawGroup(long drawId)
    {
        var groupName = $"draw_{drawId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}