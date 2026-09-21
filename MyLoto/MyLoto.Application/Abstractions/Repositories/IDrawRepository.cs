using MyLoto.Domain.Entities;

namespace MyLoto.Application.Abstractions.Repositories;

public interface IDrawRepository : IRepository<Draw>
{
    Task<IReadOnlyList<Draw>> GetDrawHistoryAsync(long lotteryId, CancellationToken ct);
    Task<IReadOnlyList<Draw>> GetActiveDrawsAsync(CancellationToken ct);
    Task<Draw?> GetDrawForBroadcastAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<Draw>> GetLiveDrawsAsync(CancellationToken ct);
}