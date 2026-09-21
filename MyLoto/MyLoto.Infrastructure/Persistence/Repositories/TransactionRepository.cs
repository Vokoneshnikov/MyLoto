namespace MyLoto.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Domain.Entities;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(LotoDbContext context) : base(context) { }

    public async Task<bool> ExistsByExternalIdAsync(string externalId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(externalId)) return false;
        
        return await Context.Set<Transaction>()
            .AnyAsync(t => t.ExternalTransactionId == externalId, ct);
    }
}