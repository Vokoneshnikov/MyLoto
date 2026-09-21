using Microsoft.EntityFrameworkCore;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Repositories;

public class LotteryRepository : Repository<Lottery>, ILotteryRepository
{
    
    public LotteryRepository(LotoDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Lottery>> GetActiveLotteriesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Lotteries
            .Where(l => !l.IsPaused)
            .ToListAsync(cancellationToken);
    }
    // Внутри твоего класса LotteryRepository : ILotteryRepository
    public async Task UpdateAsync(Lottery lottery, CancellationToken ct)
    {
        // Отмечаем сущность как измененную в DbContext
        Context.Set<Lottery>().Update(lottery);
    
        // Асинхронно сохраняем изменения в базу данных
        await Context.SaveChangesAsync(ct);
    }
}