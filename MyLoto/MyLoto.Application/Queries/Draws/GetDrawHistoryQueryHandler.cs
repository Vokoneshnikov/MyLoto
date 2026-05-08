using AutoMapper;
using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Draws;

namespace MyLoto.Application.Queries.Draws;

public class GetDrawHistoryQueryHandler 
    : IRequestHandler<GetDrawHistoryQuery, Result<IReadOnlyList<DrawHistoryDto>>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetDrawHistoryQuery> _validator; // Добавлен валидатор

    public GetDrawHistoryQueryHandler(IDrawRepository drawRepository, IMapper mapper, IValidator<GetDrawHistoryQuery> validator)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<DrawHistoryDto>>> Handle(
        GetDrawHistoryQuery request, 
        CancellationToken cancellationToken)
    {
        // Валидация запроса
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<IReadOnlyList<DrawHistoryDto>>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        // Получаем завершенные тиражи для конкретной лотереи
        var draws = await _drawRepository.GetDrawHistoryAsync(request.LotteryId, cancellationToken);

        // Маппим в список DTO
        var dtos = _mapper.Map<IReadOnlyList<DrawHistoryDto>>(draws);

        // Возвращаем результат
        return Result<IReadOnlyList<DrawHistoryDto>>.Success(dtos);
    }
}