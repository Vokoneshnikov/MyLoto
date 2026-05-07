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

    public GetActiveLotteriesQueryHandler(ILotteryRepository lotteryRepository, IMapper mapper)
    {
        _lotteryRepository = lotteryRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<LotteryDto>>> Handle(
        GetActiveLotteriesQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Используем интерфейс, который мы создали ранее
        var lotteries = await _lotteryRepository.GetActiveLotteriesAsync(cancellationToken);

        // 2. Маппим сущности Domain в DTO (настроим маппинг чуть позже)
        var dtos = _mapper.Map<IReadOnlyList<LotteryDto>>(lotteries);

        // 3. Возвращаем успешный результат
        return Result<IReadOnlyList<LotteryDto>>.Success(dtos);
    }
}