using Microsoft.EntityFrameworkCore; // Обязательно для AnyAsync()
using MyLoto.Application.Abstractions.Repositories; // Твой namespace для ITicketRepository
using MyLoto.Domain.Entities;

namespace MyLoto.Infrastructure.Persistence.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    // Добавляем собственное поле для прямого доступа к таблицам
    private readonly LotoDbContext _dbContext;

    public TicketRepository(LotoDbContext context) : base(context) 
    { 
        // Сохраняем контекст при инициализации
        _dbContext = context;
    }

    public async Task<bool> ExistsWithNumbersAsync(long drawId, List<int> numbers, CancellationToken ct)
    {
        var sortedNumbers = numbers.OrderBy(n => n).ToList();
        var count = sortedNumbers.Count;

        // Теперь обращаемся к нашему _dbContext
        return await _dbContext.Tickets
            .Where(t => t.DrawId == drawId)
            .AnyAsync(t => t.SelectedNumbers.Count == count &&
                           t.SelectedNumbers.All(sn => sortedNumbers.Contains(sn.Number)), ct);
    }
    public async Task<IReadOnlyList<Ticket>> GetByUserIdAsync(long userId, CancellationToken ct)
    {
        return await _dbContext.Tickets
            .AsNoTracking()
            .Include(t => t.SelectedNumbers) // Чтобы были числа
            .Include(t => t.Draw)            // Чтобы был тираж
            .ThenInclude(d => d.Lottery) // Чтобы было название лотереи
            .Where(t => t.OwnerId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }
}