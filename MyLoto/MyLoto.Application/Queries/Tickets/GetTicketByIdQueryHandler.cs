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

    // ЧИСТОТА: Конструктор больше не перегружен валидатором
    public GetTicketByIdQueryHandler(ITicketRepository ticketRepository, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _mapper = mapper;
    }

    public async Task<Result<TicketDto>> Handle(
        GetTicketByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // До репозитория дойдут только запросы с валидным TicketId (> 0)
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(new Error(
                "Ticket.NotFound", 
                $"Билет с ID {request.TicketId} не найден"));
        }

        var dto = _mapper.Map<TicketDto>(ticket);

        return Result<TicketDto>.Success(dto);
    }
}