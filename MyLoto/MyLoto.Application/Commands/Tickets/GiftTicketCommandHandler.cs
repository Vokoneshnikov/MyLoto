using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Commands.Tickets;

public class GiftTicketCommandHandler : IRequestHandler<GiftTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GiftTicketCommandHandler(
        ITicketRepository ticketRepository, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork, 
        IUserContext userContext)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<bool>> Handle(GiftTicketCommand request, CancellationToken ct)
    {
        // 1. Ищем билет
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, ct);
        if (ticket == null) 
            return Result<bool>.Failure(new Error("Ticket.NotFound", "Билет не найден"));

        // 2. Проверяем, что даритель — действительно текущий владелец билета
        if (ticket.OwnerId != _userContext.UserId)
            return Result<bool>.Failure(new Error("Ticket.AccessDenied", "Вы не можете подарить чужой билет"));

        // 3. Ищем счастливчика (совмещаем проверку существования и получение данных)
        var recipient = await _userRepository.GetByLoginAsync(request.RecipientLogin, ct);
        if (recipient == null)
        {
            return Result<bool>.Failure(new Error(
                "Recipient.NotFound", 
                $"Пользователь с логином '{request.RecipientLogin}' не найден"));
        }

        // 4. Проверяем нарциссизм :)
        if (recipient.Id == _userContext.UserId)
        {
            return Result<bool>.Failure(new Error(
                "Gift.Self", 
                "Подарить билет самому себе — это просто оставить его в своем кармане :)"));
        }

        // 5. Переписываем владельца билета
        ticket.OwnerId = recipient.Id;

        await _unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}