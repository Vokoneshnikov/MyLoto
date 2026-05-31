using MediatR;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

public class GetUserTicketsQueryHandler 
    : IRequestHandler<GetUserTicketsQuery, Result<IReadOnlyList<UserTicketDto>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContext _userContext;

    // ЧИСТОТА: Больше никакого IValidator в конструкторе!
    public GetUserTicketsQueryHandler(
        ITicketRepository ticketRepository, 
        IUserContext userContext)
    {
        _ticketRepository = ticketRepository;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<UserTicketDto>>> Handle(
        GetUserTicketsQuery request, 
        CancellationToken cancellationToken)
    {
        // Контекст авторизации: забираем ID текущего пользователя
        var userId = _userContext.UserId;

        // Запрос к репозиторию, который сразу вернет отфильтрованные DTO.
        // Сюда мы гарантированно зайдем с валидной комбинацией флагов IsArchive и IsWon.
        var dtos = await _ticketRepository.GetFilteredTicketsDtoAsync(
            userId, 
            request.IsArchive, 
            request.IsWon, 
            cancellationToken);

        return Result<IReadOnlyList<UserTicketDto>>.Success(dtos);
    }
}