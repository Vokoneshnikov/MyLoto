using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public class GetDrawHistoryQueryHandler 
    : IRequestHandler<GetDrawHistoryQuery, Result<IReadOnlyList<DrawHistoryDto>>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IMapper _mapper;

    public GetDrawHistoryQueryHandler(IDrawRepository drawRepository, IMapper mapper)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<DrawHistoryDto>>> Handle(
        GetDrawHistoryQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Получаем завершенные тиражи для конкретной лотереи
        var draws = await _drawRepository.GetDrawHistoryAsync(request.LotteryId, cancellationToken);

        // 2. Маппим в список DTO
        var dtos = _mapper.Map<IReadOnlyList<DrawHistoryDto>>(draws);

        // 3. Возвращаем результат
        return Result<IReadOnlyList<DrawHistoryDto>>.Success(dtos);
    }
}