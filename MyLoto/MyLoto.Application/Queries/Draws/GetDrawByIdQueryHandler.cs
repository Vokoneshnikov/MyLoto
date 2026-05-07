using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Draws;

public class GetDrawByIdQueryHandler : IRequestHandler<GetDrawByIdQuery, Result<DrawDto>>
{
    private readonly IDrawRepository _drawRepository;
    private readonly IMapper _mapper;

    public GetDrawByIdQueryHandler(IDrawRepository drawRepository, IMapper mapper)
    {
        _drawRepository = drawRepository;
        _mapper = mapper;
    }

    public async Task<Result<DrawDto>> Handle(GetDrawByIdQuery request, CancellationToken ct)
    {
        var draw = await _drawRepository.GetByIdAsync(request.DrawId, ct);

        if (draw is null)
            return Result<DrawDto>.Failure(new Error("Draw.NotFound", "Тираж не найден"));

        var dto = _mapper.Map<DrawDto>(draw);
        return Result<DrawDto>.Success(dto);
    }
}