using MyLoto.Application.Abstractions;

namespace MyLoto.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly LotoDbContext _context;

    public UnitOfWork(LotoDbContext context) => _context = context;
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}