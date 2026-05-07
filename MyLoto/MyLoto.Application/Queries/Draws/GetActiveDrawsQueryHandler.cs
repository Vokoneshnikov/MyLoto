using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public class GetActiveDrawsQueryHandler : IRequestHandler<GetActiveDrawsQuery, Result<IReadOnlyList<DrawDto>>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IMapper _mapper;

    public GetActiveDrawsQueryHandler(IDrawRepository drawRepository, IMapper mapper)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<DrawDto>>> Handle(GetActiveDrawsQuery request, CancellationToken ct)
    {
        var activeDraws = await _drawRepository.GetActiveDrawsAsync(ct);
        var dtos = _mapper.Map<IReadOnlyList<DrawDto>>(activeDraws);

        return Result<IReadOnlyList<DrawDto>>.Success(dtos);
    }
}