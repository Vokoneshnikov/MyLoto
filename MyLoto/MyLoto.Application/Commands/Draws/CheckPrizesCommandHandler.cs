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
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        draw.Status = DrawStatus.Checking;
        await _unitOfWork.SaveChangesAsync(ct);
        
        var drawnNumbers = draw.WinningNumbers
            .OrderBy(wn => wn.Order)
            .Select(wn => wn.Number)
            .ToList();

        foreach (var ticket in draw.Tickets)
        {
            ticket.WinAmount = CalculateTicketWin(ticket, draw.Lottery, drawnNumbers);
            ticket.IsChecked = true;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        // Переходим к выплатам
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

        if (matchCount == lottery.NumbersToChoose && matchCount > 0) return lottery.AccumulatedJackpot;

        var tier = lottery.PrizeTiers.FirstOrDefault(t => 
            t.RuleType == PrizeTierRuleType.MatchedNumbers && t.ConditionValue == matchCount);

        return tier != null ? lottery.TicketPrice * tier.RewardMultiplier : 0;
    }
    
    private decimal CalculateBingo(Ticket ticket, BingoLottery lottery, List<int> drawnNumbers)
    {
        var ticketNumbers = ticket.SelectedNumbers.Select(sn => sn.Number).ToList();
        int n = lottery.Columns;
        var firstNNumbers = drawnNumbers.Take(n).ToHashSet();

        for (int r = 0; r < lottery.Rows; r++)
        {
            var rowNumbers = ticketNumbers.Skip(r * lottery.Columns).Take(lottery.Columns).ToList();
            if (rowNumbers.All(num => firstNNumbers.Contains(num))) return lottery.AccumulatedJackpot;
        }

        var allTicketNumbersSet = ticketNumbers.ToHashSet();
        int stepsToWin = 0;
        int matchedCount = 0;

        for (int i = 0; i < drawnNumbers.Count; i++)
        {
            if (allTicketNumbersSet.Contains(drawnNumbers[i])) matchedCount++;
            if (matchedCount == allTicketNumbersSet.Count) { stepsToWin = i + 1; break; }
        }

        if (stepsToWin == 0) return 0;

        var tier = lottery.PrizeTiers.FirstOrDefault(t => 
            t.RuleType == PrizeTierRuleType.ClosedAtBall && t.ConditionValue == stepsToWin);

        return tier != null ? lottery.TicketPrice * tier.RewardMultiplier : 0;
    }
}