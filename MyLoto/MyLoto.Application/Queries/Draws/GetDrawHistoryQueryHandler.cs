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

    // ЧИСТОТА: Убрали валидатор из конструктора
    public GetDrawHistoryQueryHandler(IDrawRepository drawRepository, IMapper mapper)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<DrawHistoryDto>>> Handle(
        GetDrawHistoryQuery request, 
        CancellationToken cancellationToken)
    {
        // Метод выполнится только если LotteryId > 0
        var draws = await _drawRepository.GetDrawHistoryAsync(request.LotteryId, cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<DrawHistoryDto>>(draws);

        return Result<IReadOnlyList<DrawHistoryDto>>.Success(dtos);
    }
}