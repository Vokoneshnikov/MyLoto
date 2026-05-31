using AutoMapper;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;

namespace MyLoto.Application.Queries.Lotteries;

public class GetAllLotteriesQueryHandler : IRequestHandler<GetAllLotteriesQuery, Result<List<LotteryDto>>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IMapper _mapper;

    public GetAllLotteriesQueryHandler(ILotteryRepository lotteryRepository, IMapper mapper)
    {
        _lotteryRepository = lotteryRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<LotteryDto>>> Handle(GetAllLotteriesQuery request, CancellationToken ct)
    {
        // 1. Получаем список всех доменных сущностей из репозитория
        var lotteries = await _lotteryRepository.GetAllAsync(ct); 
        
        // 2. Маппим коллекцию в DTO (настройки полиморфизма берутся из MappingProfile)
        var dtos = _mapper.Map<List<LotteryDto>>(lotteries);
        
        return Result<List<LotteryDto>>.Success(dtos); 
    }
}