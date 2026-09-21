using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Lotteries;

public class GetActiveLotteriesQueryHandler 
    : IRequestHandler<GetActiveLotteriesQuery, Result<IReadOnlyList<LotteryDto>>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IMapper _mapper;

    // ЧИСТОТА: Больше никакой инжекции IValidator в конструкторе
    public GetActiveLotteriesQueryHandler(ILotteryRepository lotteryRepository, IMapper mapper)
    {
        _lotteryRepository = lotteryRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<LotteryDto>>> Handle(
        GetActiveLotteriesQuery request, 
        CancellationToken cancellationToken)
    {
        // Сюда попадаем только при успешной валидации на уровне MediatR Pipeline
        var lotteries = await _lotteryRepository.GetActiveLotteriesAsync(cancellationToken);

        // Маппим сущности Domain в DTO
        var dtos = _mapper.Map<IReadOnlyList<LotteryDto>>(lotteries);

        return Result<IReadOnlyList<LotteryDto>>.Success(dtos);
    }
}