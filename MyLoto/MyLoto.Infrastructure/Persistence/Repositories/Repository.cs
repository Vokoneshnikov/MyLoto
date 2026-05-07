using MyLoto.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyLoto.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly LotoDbContext Context;

    public Repository(LotoDbContext context)
    {
        Context = context;
    }

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Set<T>().ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await Context.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void Delete(T entity) => Context.Set<T>().Remove(entity);
}