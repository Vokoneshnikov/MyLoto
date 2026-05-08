using AutoMapper;
using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Draws;

namespace MyLoto.Application.Queries.Draws;

public class GetDrawByIdQueryHandler : IRequestHandler<GetDrawByIdQuery, Result<DrawDto>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetDrawByIdQuery> _validator; // Добавляем валидатор

    public GetDrawByIdQueryHandler(IDrawRepository drawRepository, IMapper mapper, IValidator<GetDrawByIdQuery> validator)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<DrawDto>> Handle(GetDrawByIdQuery request, CancellationToken ct)
    {
        // Валидируем запрос
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<DrawDto>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);

        if (draw is null)
            return Result<DrawDto>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        var dto = _mapper.Map<DrawDto>(draw);
        return Result<DrawDto>.Success(dto);
    }
}