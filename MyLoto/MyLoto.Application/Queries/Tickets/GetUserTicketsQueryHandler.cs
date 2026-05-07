using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

public class GetUserTicketsQueryHandler 
    : IRequestHandler<GetUserTicketsQuery, Result<IReadOnlyList<UserTicketDto>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMapper _mapper;

    public GetUserTicketsQueryHandler(ITicketRepository ticketRepository, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<UserTicketDto>>> Handle(
        GetUserTicketsQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Получаем билеты пользователя через репозиторий
        // (Убедись, что в ITicketRepository есть метод GetByUserIdAsync)
        var tickets = await _ticketRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        // 2. Маппим сущности Ticket в UserTicketDto
        var dtos = _mapper.Map<IReadOnlyList<UserTicketDto>>(tickets);

        // 3. Возвращаем результат
        return Result<IReadOnlyList<UserTicketDto>>.Success(dtos);
    }
}