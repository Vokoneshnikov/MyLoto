namespace MyLoto.Application.Abstractions.Repositories;

using MyLoto.Domain.Entities;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<bool> ExistsByExternalIdAsync(string externalId, CancellationToken ct);
}