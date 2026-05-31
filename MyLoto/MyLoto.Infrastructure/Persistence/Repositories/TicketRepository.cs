using Microsoft.EntityFrameworkCore;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Queries.Tickets;
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    private readonly LotoDbContext _dbContext;

    public TicketRepository(LotoDbContext context) : base(context) 
    { 
        _dbContext = context;
    }

    public async Task<bool> ExistsWithNumbersAsync(long drawId, List<int> numbers, CancellationToken ct)
    {
        var sortedNumbers = numbers.OrderBy(n => n).ToList();
        var count = sortedNumbers.Count;

        return await _dbContext.Tickets
            .Where(t => t.DrawId == drawId)
            .AnyAsync(t => t.SelectedNumbers.Count == count &&
                           t.SelectedNumbers.All(sn => sortedNumbers.Contains(sn.Number)), ct);
    }

    public async Task<IReadOnlyList<Ticket>> GetByUserIdAsync(long userId, CancellationToken ct)
    {
        return await _dbContext.Tickets
            .AsNoTracking()
            .Include(t => t.SelectedNumbers)
            .Include(t => t.Draw)
            .ThenInclude(d => d.Lottery)
            .Where(t => t.OwnerId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    // 🔥 ИСПРАВЛЕНО: Метод фильтрации и проекции прямо в базу данных
    public async Task<IReadOnlyList<UserTicketDto>> GetFilteredTicketsDtoAsync(
        long userId, 
        bool isArchive, 
        bool? isWon, 
        CancellationToken ct)
    {
        // Формируем базовый запрос с фильтром по юзеру и статусу проверки билета
        var query = _dbContext.Tickets
            .AsNoTracking()
            .Where(t => t.OwnerId == userId && t.IsChecked == isArchive);

        // Фильтр выиграл/проиграл (только для архивных)
        if (isArchive && isWon.HasValue)
        {
            query = isWon.Value 
                ? query.Where(t => t.WinAmount > 0) 
                : query.Where(t => t.WinAmount == 0);
        }

        // Выполняем сортировку и маппинг прямо на стороне СУБД
        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new UserTicketDto(
                t.Id,
                t.DrawId,
                t.SelectedNumbers.Select(n => n.Number).ToList(),
                t.IsChecked,
                t.WinAmount,
                t.Draw.Status.ToString(),
                t.Draw.WinningNumbers
                    .OrderBy(wn => wn.Order)
                    .Select(wn => wn.Number)
                    .ToList()
            ))
            .ToListAsync(ct);
    }
}