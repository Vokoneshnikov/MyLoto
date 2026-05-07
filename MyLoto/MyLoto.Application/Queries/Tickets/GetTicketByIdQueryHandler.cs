using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

public class GetTicketByIdQueryHandler 
    : IRequestHandler<GetTicketByIdQuery, Result<TicketDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMapper _mapper;

    public GetTicketByIdQueryHandler(ITicketRepository ticketRepository, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _mapper = mapper;
    }

    public async Task<Result<TicketDto>> Handle(
        GetTicketByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Ищем билет в базе. 
        // Важно: в реализации репозитория этот метод должен включать SelectedNumbers
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(new Error(
                "Ticket.NotFound", 
                $"Билет с ID {request.TicketId} не найден"));
        }

        // 2. Маппим сущность в DTO
        var dto = _mapper.Map<TicketDto>(ticket);

        // 3. Возвращаем результат
        return Result<TicketDto>.Success(dto);
    }
}