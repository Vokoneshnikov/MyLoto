namespace MyLoto.Application.Abstractions.Repositories;

using MyLoto.Domain.Entities;

public interface ILotteryRepository : IRepository<Lottery>
{
    Task<IReadOnlyList<Lottery>> GetActiveLotteriesAsync(CancellationToken cancellationToken = default);
}