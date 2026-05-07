using Microsoft.EntityFrameworkCore;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(LotoDbContext context) : base(context) { }

    public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await Context.Users
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
    }

    public async Task<bool> IsLoginUniqueAsync(string login, CancellationToken cancellationToken = default)
    {
        return !await Context.Users
            .AnyAsync(u => u.Login == login, cancellationToken);
    }

    public async Task<User?> GetByIdWithTicketsAsync(long id, CancellationToken ct)
    {
        return await Context.Users
            .AsNoTracking()
            .Include(u => u.OwnedTickets)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}