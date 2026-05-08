using AutoMapper;
using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Validators.Draws;

namespace MyLoto.Application.Queries.Draws;

public class GetActiveDrawsQueryHandler : IRequestHandler<GetActiveDrawsQuery, Result<IReadOnlyList<DrawDto>>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetActiveDrawsQuery> _validator;

    public GetActiveDrawsQueryHandler(IDrawRepository drawRepository, IMapper mapper, IValidator<GetActiveDrawsQuery> validator)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<DrawDto>>> Handle(GetActiveDrawsQuery request, CancellationToken ct)
    {
        // Валидация запроса
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<IReadOnlyList<DrawDto>>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        // Получаем активные тиражи
        var activeDraws = await _drawRepository.GetActiveDrawsAsync(ct);
        var dtos = _mapper.Map<IReadOnlyList<DrawDto>>(activeDraws);

        return Result<IReadOnlyList<DrawDto>>.Success(dtos);
    }
}