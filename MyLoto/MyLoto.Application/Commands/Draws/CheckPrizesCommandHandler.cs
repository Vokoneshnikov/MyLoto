using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class CheckPrizesCommandHandler : IRequestHandler<CheckPrizesCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CheckPrizesCommandHandler(IDrawRepository drawRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _drawRepository = drawRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<Unit>> Handle(CheckPrizesCommand request, CancellationToken ct)
    {
        // До репозитория дойдут только валидные идентификаторы (> 0)
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // 1. Фиксируем статус проверки, чтобы никто не изменил состояние тиража параллельно
        draw.Status = DrawStatus.Checking;
        await _unitOfWork.SaveChangesAsync(ct);
        
        var drawnNumbers = draw.WinningNumbers
            .OrderBy(wn => wn.Order)
            .Select(wn => wn.Number)
            .ToList();

        // 2. Расчёт выигрышей для каждого купленного билета в тираже
        foreach (var ticket in draw.Tickets)
        {
            ticket.WinAmount = CalculateTicketWin(ticket, draw.Lottery, drawnNumbers);
            ticket.IsChecked = true;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        // 3. Передаем управление команде распределения и выплаты призов
        return await _mediator.Send(new DistributePrizesCommand(draw.Id), ct);
    }

    private decimal CalculateTicketWin(Ticket ticket, Lottery lottery, List<int> drawnNumbers)
    {
        return lottery switch
        {
            KOutOfNLottery kLottery => CalculateKOutOfN(ticket, kLottery, drawnNumbers),
            BingoLottery bLottery => CalculateBingo(ticket, bLottery, drawnNumbers),
            _ => 0
        };
    }

    private decimal CalculateKOutOfN(Ticket ticket, KOutOfNLottery lottery, List<int> drawnNumbers)
    {
        var selectedNumbers = ticket.SelectedNumbers.Select(sn => sn.Number).ToHashSet();
        var matchCount = drawnNumbers.Count(n => selectedNumbers.Contains(n));

        // Если угаданы абсолютно все числа — это Джекпот!
        if (matchCount == lottery.NumbersToChoose && matchCount > 0) 
            return lottery.AccumulatedJackpot;

        // Иначе ищем подходящую категорию выигрыша по количеству совпадений
        var tier = lottery.PrizeTiers.FirstOrDefault(t => 
            t.RuleType == PrizeTierRuleType.MatchedNumbers && t.ConditionValue == matchCount);

        return tier != null ? lottery.TicketPrice * tier.RewardMultiplier : 0;
    }
    
    private decimal CalculateBingo(Ticket ticket, BingoLottery lottery, List<int> drawnNumbers)
    {
        var ticketNumbers = ticket.SelectedNumbers.Select(sn => sn.Number).ToList();
        int n = lottery.Columns;
        var firstNNumbers = drawnNumbers.Take(n).ToHashSet();

        // Проверяем классическое правило Бинго: закрытие любой горизонтальной строки за первые N шаров
        for (int r = 0; r < lottery.Rows; r++)
        {
            var rowNumbers = ticketNumbers.Skip(r * lottery.Columns).Take(lottery.Columns).ToList();
            if (rowNumbers.All(num => firstNNumbers.Contains(num))) 
                return lottery.AccumulatedJackpot;
        }

        var allTicketNumbersSet = ticketNumbers.ToHashSet();
        int stepsToWin = 0;
        int matchedCount = 0;

        // Считаем, на каком конкретно шаре (шаге) у пользователя закрылся весь билет
        for (int i = 0; i < drawnNumbers.Count; i++)
        {
            if (allTicketNumbersSet.Contains(drawnNumbers[i])) matchedCount++;
            if (matchedCount == allTicketNumbersSet.Count) 
            { 
                stepsToWin = i + 1; 
                break; 
            }
        }

        if (stepsToWin == 0) return 0;

        // Проверяем, положена ли награда за закрытие билета на полученном шаге
        var tier = lottery.PrizeTiers.FirstOrDefault(t => 
            t.RuleType == PrizeTierRuleType.ClosedAtBall && t.ConditionValue == stepsToWin);

        return tier != null ? lottery.TicketPrice * tier.RewardMultiplier : 0;
    }
}