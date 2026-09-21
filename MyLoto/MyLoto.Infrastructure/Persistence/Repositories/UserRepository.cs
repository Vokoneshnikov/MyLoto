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
            .Include(u => u.ExtraInfo)  // Включаем данные из UserExtraInfo
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
            .Include(u => u.OwnedTickets)
            .Include(u => u.ExtraInfo)  // Включаем данные из UserExtraInfo
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
    public async Task<UserExtraInfo?> GetExtraInfoAsync(long userId, CancellationToken ct)
    {
        return await Context.UserExtraInfo
            .FirstOrDefaultAsync(e => e.UserId == userId, ct);
    }
    // В UserRepository.cs
    public async Task<User?> GetWithExtraInfoAsync(long userId, CancellationToken ct)
    {
        return await Context.Users
            .Include(u => u.ExtraInfo)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }
}