using AutoMapper;
using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Queries.Tickets;

public class GetTicketByIdQueryHandler 
    : IRequestHandler<GetTicketByIdQuery, Result<TicketDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetTicketByIdQuery> _validator; // Вставлен валидатор

    public GetTicketByIdQueryHandler(
        ITicketRepository ticketRepository, 
        IMapper mapper, 
        IValidator<GetTicketByIdQuery> validator)
    {
        _ticketRepository = ticketRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<TicketDto>> Handle(
        GetTicketByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // Валидация запроса
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<TicketDto>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        // Ищем билет в базе
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(new Error(
                "Ticket.NotFound", 
                $"Билет с ID {request.TicketId} не найден"));
        }

        // Маппим сущность в DTO
        var dto = _mapper.Map<TicketDto>(ticket);

        // Возвращаем результат
        return Result<TicketDto>.Success(dto);
    }
}