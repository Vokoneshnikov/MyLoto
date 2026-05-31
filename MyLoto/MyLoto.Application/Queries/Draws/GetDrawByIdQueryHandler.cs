using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public class GetDrawByIdQueryHandler : IRequestHandler<GetDrawByIdQuery, Result<DrawDto>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IMapper _mapper;

    // ЧИСТОТА: Убрали валидатор из конструктора
    public GetDrawByIdQueryHandler(IDrawRepository drawRepository, IMapper mapper)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
    }

    public async Task<Result<DrawDto>> Handle(GetDrawByIdQuery request, CancellationToken ct)
    {
        // Запрос уйдет в базу только если DrawId прошел валидацию конвейера (> 0)
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);

        // Проверка существования — это бизнес-правило, оставляем в хандлере
        if (draw is null)
            return Result<DrawDto>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        var dto = _mapper.Map<DrawDto>(draw);
        return Result<DrawDto>.Success(dto);
    }
}