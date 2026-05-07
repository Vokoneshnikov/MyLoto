using MyLoto.Domain.Entities;

namespace MyLoto.Application.Abstractions.Repositories;

public interface IDrawRepository : IRepository<Draw>
{
    Task<IReadOnlyList<Draw>> GetDrawHistoryAsync(long lotteryId, CancellationToken ct);
    Task<IReadOnlyList<Draw>> GetActiveDrawsAsync(CancellationToken ct);
}