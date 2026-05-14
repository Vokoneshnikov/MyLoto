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
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);
        if (draw == null) return Result<Unit>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        var winningTickets = draw.Tickets.Where(t => t.WinAmount > 0).ToList();

        foreach (var ticket in winningTickets)
        {
            // Начисляем деньги пользователю
            ticket.Owner.DepositMoney(ticket.WinAmount);

            // Создаем транзакцию (используем string.Format для обхода ошибки интерполяции)
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

        await _unitOfWork.SaveChangesAsync(ct);

        // Переходим к финализации тиража
        return await _mediator.Send(new CompleteDrawCommand(draw.Id), ct);
    }
}