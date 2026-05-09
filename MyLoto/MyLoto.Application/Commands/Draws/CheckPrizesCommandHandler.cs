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

    public CheckPrizesCommandHandler(IDrawRepository drawRepository, IUnitOfWork unitOfWork)
    {
        _drawRepository = drawRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(CheckPrizesCommand request, CancellationToken ct)
    {
        // ВАЖНО: Репозиторий должен подтягивать Lottery, PrizeTiers и SelectedNumbers в билетах
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        
        if (draw == null)
            return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        if (draw.Status != DrawStatus.Checking)
            return Result<Unit>.Failure(new Error("Draw.InvalidStatus", "Тираж должен быть в статусе проверки"));

        // Получаем упорядоченный список выпавших чисел (важно для Бинго)
        var drawnNumbers = draw.WinningNumbers
            .OrderBy(wn => wn.Order)
            .Select(wn => wn.Number)
            .ToList();

        foreach (var ticket in draw.Tickets)
        {
            ticket.WinAmount = CalculateTicketWin(ticket, draw.Lottery, drawnNumbers);
            ticket.IsChecked = true;
        }

        // После проверки переводим тираж в статус завершенного
        draw.Status = DrawStatus.Completed;

        await _unitOfWork.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
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

        if (matchCount == lottery.NumbersToChoose && matchCount > 0)
        {
            return lottery.AccumulatedJackpot;
        }
        // Ищем подходящий тир (правило)
        var tier = lottery.PrizeTiers.FirstOrDefault(t => 
            t.RuleType == PrizeTierRuleType.MatchedNumbers && t.ConditionValue == matchCount);

        return tier != null ? lottery.TicketPrice * tier.RewardMultiplier : 0;
    }

    // private decimal CalculateBingo(Ticket ticket, BingoLottery lottery, List<int> drawnNumbers)
    // {
    //     var ticketNumbers = ticket.SelectedNumbers.Select(sn => sn.Number).ToHashSet();
    //     
    //     // Находим, на каком шаге (индексе) были закрыты все числа билета
    //     int stepsToWin = 0;
    //     int matchedCount = 0;
    //
    //     for (int i = 0; i < drawnNumbers.Count; i++)
    //     {
    //         if (ticketNumbers.Contains(drawnNumbers[i]))
    //         {
    //             matchedCount++;
    //         }
    //
    //         if (matchedCount == ticketNumbers.Count)
    //         {
    //             stepsToWin = i + 1; // Номер шара (от 1 до N)
    //             break;
    //         }
    //     }
    //
    //     if (stepsToWin == 0) return 0; // Билет не закрылся вовсе
    //
    //     // 1. Проверка на Джекпот (Этап 1: закрыл всё до n-го шара включительно)
    //     if (stepsToWin <= lottery.JackpotThreshold)
    //     {
    //         return lottery.AccumulatedJackpot;
    //     }
    //
    //     // 2. Основной розыгрыш: ищем тир по номеру шара закрытия
    //     var tier = lottery.PrizeTiers.FirstOrDefault(t => 
    //         t.RuleType == PrizeTierRuleType.ClosedAtBall && t.ConditionValue == stepsToWin);
    //
    //     return tier != null ? lottery.TicketPrice * tier.RewardMultiplier : 0;
    // }
    
    private decimal CalculateBingo(Ticket ticket, BingoLottery lottery, List<int> drawnNumbers)
    {
        // Преобразуем выбранные числа в список (предполагаем, что они хранятся в порядке добавления: строка 1, строка 2...)
        var ticketNumbers = ticket.SelectedNumbers.Select(sn => sn.Number).ToList();
    
        // --- 1. ПРОВЕРКА НА ДЖЕКПОТ (Горизонталь за n ходов) ---
        // Согласно твоему условию n = lottery.Columns
        int n = lottery.Columns;
        var firstNNumbers = drawnNumbers.Take(n).ToHashSet();
        bool hasJackpot = false;

        for (int r = 0; r < lottery.Rows; r++)
        {
            // Извлекаем числа текущей строки
            var rowNumbers = ticketNumbers.Skip(r * lottery.Columns).Take(lottery.Columns).ToList();
        
            // Если все числа в этой строке присутствуют среди первых n выпавших шаров
            if (rowNumbers.All(num => firstNNumbers.Contains(num)))
            {
                hasJackpot = true;
                break; 
            }
        }

        if (hasJackpot)
        {
            return lottery.AccumulatedJackpot;
        }

        // --- 2. ОСНОВНОЙ РОЗЫГРЫШ (Весь билет) ---
        // Если джекпот не сорван, считаем за сколько ходов закрылся весь билет
        var allTicketNumbersSet = ticketNumbers.ToHashSet();
        int stepsToWin = 0;
        int matchedCount = 0;

        for (int i = 0; i < drawnNumbers.Count; i++)
        {
            if (allTicketNumbersSet.Contains(drawnNumbers[i]))
            {
                matchedCount++;
            }

            if (matchedCount == allTicketNumbersSet.Count)
            {
                stepsToWin = i + 1;
                break;
            }
        }

        if (stepsToWin == 0) return 0;

        // Ищем обычный приз по номеру шара закрытия в PrizeTiers
        var tier = lottery.PrizeTiers.FirstOrDefault(t => 
            t.RuleType == PrizeTierRuleType.ClosedAtBall && t.ConditionValue == stepsToWin);

        return tier != null ? lottery.TicketPrice * tier.RewardMultiplier : 0;
    }
}