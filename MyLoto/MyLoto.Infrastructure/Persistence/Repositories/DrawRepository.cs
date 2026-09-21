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
            .Where(d => (d.Status == DrawStatus.Pending))  // Только те, что не завершены
            .ToListAsync(ct);
    }

    public async Task<Draw?> GetByIdAsync(long id, CancellationToken ct)
    {
        return await Context.Draws
            .Include(d => d.Lottery)
            .ThenInclude(l => l.PrizeTiers)
            .Include(d => d.WinningNumbers)
            .Include(d => d.Tickets)
            .ThenInclude(t => t.SelectedNumbers)
            .Include(d => d.Tickets)
            .ThenInclude(t => t.Owner)
            .AsSplitQuery() // Помогает избежать гигантских джоинов
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }
    public async Task<Draw?> GetDrawForBroadcastAsync(long id, CancellationToken ct)
    {
        return await Context.Draws
            .Include(d => d.Lottery)
            .Include(d => d.WinningNumbers)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }
    public async Task<IReadOnlyList<Draw>> GetLiveDrawsAsync(CancellationToken ct)
    {
        return await Context.Draws
            .AsNoTracking()
            .Include(d => d.Lottery)
            .Where(d => d.Status == DrawStatus.InProgress)
            .ToListAsync(ct);
    }
}