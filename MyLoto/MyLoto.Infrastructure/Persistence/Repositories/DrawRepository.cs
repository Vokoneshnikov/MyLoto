using Microsoft.EntityFrameworkCore;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Infrastructure.Persistence.Repositories;

public class DrawRepository : Repository<Draw>, IDrawRepository
{
    public DrawRepository(LotoDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Draw>> GetDrawHistoryAsync(long lotteryId, CancellationToken ct)
    {
        return await Context.Draws
            .AsNoTracking()
            .Include(d => d.WinningNumbers) // Предполагаем, что связь называется так
            .Where(d => d.LotteryId == lotteryId && d.Status == DrawStatus.Completed) // Или проверяем по статусу Status == DrawStatus.Finished
            .OrderByDescending(d => d.ScheduledStartTime)
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<Draw>> GetActiveDrawsAsync(CancellationToken ct)
    {
        return await Context.Draws
            .AsNoTracking()
            .Include(d => d.Lottery) // Обязательно подгружаем лотерею для названия и цены
            .Where(d => !(d.Status == DrawStatus.Completed))  // Только те, что не завершены
            .ToListAsync(ct);
    }

    public async Task<Draw?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Context.Draws
            .Include(d => d.Lottery)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}