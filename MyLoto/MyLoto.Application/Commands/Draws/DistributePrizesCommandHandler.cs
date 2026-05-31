using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Draws;

public class DistributePrizesCommandHandler : IRequestHandler<DistributePrizesCommand, Result<Unit>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DistributePrizesCommandHandler(IDrawRepository drawRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _drawRepository = drawRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<Unit>> Handle(DistributePrizesCommand request, CancellationToken ct)
    {
        // До репозитория доберутся только валидные ID (> 0)
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        // Выбираем билеты, которым алгоритм розыгрыша уже насчитал WinAmount
        var winningTickets = draw.Tickets.Where(t => t.WinAmount > 0).ToList();

        foreach (var ticket in winningTickets)
        {
            // Начисляем выигрыш на баланс пользователя (доменная логика)
            ticket.Owner.DepositMoney(ticket.WinAmount);

            // Формируем финансовую транзакцию для истории и аудита
            var transaction = new Transaction
            {
                UserId = ticket.OwnerId,
                User = ticket.Owner,
                Amount = ticket.WinAmount,
                Type = TransactionType.Win,
                TicketId = ticket.Id,
                Description = string.Format("Выигрыш в тираже #{0} ({1})", 
                    draw.Id.ToString(), 
                    draw.Lottery?.Name ?? "Loto")
            };

            ticket.Owner.Transactions.Add(transaction);
        }

        // Фиксируем все начисления и транзакции в рамках единой UoW-сессии
        await _unitOfWork.SaveChangesAsync(ct);

        // Передаем эстафету следующему шагу — финальному закрытию тиража
        return await _mediator.Send(new CompleteDrawCommand(draw.Id), ct);
    }
}