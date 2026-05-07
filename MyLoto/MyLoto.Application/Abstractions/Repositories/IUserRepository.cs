namespace MyLoto.Application.Abstractions.Repositories;

using MyLoto.Domain.Entities;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<bool> IsLoginUniqueAsync(string login, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithTicketsAsync(long id, CancellationToken ct);
}