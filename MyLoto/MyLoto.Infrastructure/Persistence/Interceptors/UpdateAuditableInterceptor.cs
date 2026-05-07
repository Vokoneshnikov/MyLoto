using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyLoto.Domain;

namespace MyLoto.Infrastructure.Persistence.Interceptors;

public sealed class UpdateAuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) 
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        // Находим все сущности, которые наследуются от BaseEntity
        var entries = eventData.Context.ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            // Если мы только что создали запись
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            // Если мы обновили существующую запись
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}