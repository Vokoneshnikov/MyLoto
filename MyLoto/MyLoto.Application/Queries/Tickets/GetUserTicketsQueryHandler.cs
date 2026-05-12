using AutoMapper;
using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Queries.Tickets;

public class GetUserTicketsQueryHandler 
    : IRequestHandler<GetUserTicketsQuery, Result<IReadOnlyList<UserTicketDto>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetUserTicketsQuery> _validator;
    private readonly IUserContext _userContext;

    public GetUserTicketsQueryHandler(
        ITicketRepository ticketRepository, 
        IMapper mapper, 
        IValidator<GetUserTicketsQuery> validator,
        IUserContext userContext)
    {
        _ticketRepository = ticketRepository;
        _mapper = mapper;
        _validator = validator;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<UserTicketDto>>> Handle(
        GetUserTicketsQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<IReadOnlyList<UserTicketDto>>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        // Получаем билеты пользователя через репозиторий
        var tickets = await _ticketRepository.GetByUserIdAsync(userId, cancellationToken);

        // Маппим сущности Ticket в UserTicketDto
        var dtos = _mapper.Map<IReadOnlyList<UserTicketDto>>(tickets);

        // Возвращаем результат
        return Result<IReadOnlyList<UserTicketDto>>.Success(dtos);
    }
}