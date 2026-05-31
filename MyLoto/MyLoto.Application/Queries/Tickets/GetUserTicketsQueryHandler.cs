using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Tickets;

public class GetUserTicketsQueryHandler 
    : IRequestHandler<GetUserTicketsQuery, Result<IReadOnlyList<UserTicketDto>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IValidator<GetUserTicketsQuery> _validator;
    private readonly IUserContext _userContext;

    public GetUserTicketsQueryHandler(
        ITicketRepository ticketRepository, 
        IValidator<GetUserTicketsQuery> validator,
        IUserContext userContext)
    {
        _ticketRepository = ticketRepository;
        _validator = validator;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<UserTicketDto>>> Handle(
        GetUserTicketsQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Валидация входных параметров
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<IReadOnlyList<UserTicketDto>>.Failure(
                new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        // 2. Безопасное получение ID из контекста авторизации
        var userId = _userContext.UserId;

        // 3. Запрос к репозиторию, который сразу вернет готовые DTO
        var dtos = await _ticketRepository.GetFilteredTicketsDtoAsync(
            userId, 
            request.IsArchive, 
            request.IsWon, 
            cancellationToken);

        return Result<IReadOnlyList<UserTicketDto>>.Success(dtos);
    }
}