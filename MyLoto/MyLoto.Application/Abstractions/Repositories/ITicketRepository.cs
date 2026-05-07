using MyLoto.Domain.Entities;

namespace MyLoto.Application.Abstractions.Repositories;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<bool> ExistsWithNumbersAsync(long drawId, List<int> numbers, CancellationToken ct);
    Task<IReadOnlyList<Ticket>> GetByUserIdAsync(long userId, CancellationToken ct);
}