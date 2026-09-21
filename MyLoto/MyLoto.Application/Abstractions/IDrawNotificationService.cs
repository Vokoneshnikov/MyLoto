namespace MyLoto.Application.Abstractions;

public interface IDrawNotificationService
{
    Task SendNumberAsync(long drawId, int number, int order, CancellationToken ct);
    Task SendDrawFinishedAsync(long drawId, CancellationToken ct);
}