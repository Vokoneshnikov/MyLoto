using MediatR;
using AutoMapper; // Не забудь добавить импорт AutoMapper
using MyLoto.Application.Common;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Queries.Lotteries;

public class GetAllLotteriesQueryHandler : IRequestHandler<GetAllLotteriesQuery, Result<List<LotteryDto>>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IMapper _mapper; // Внедряем маппер

    public GetAllLotteriesQueryHandler(ILotteryRepository lotteryRepository, IMapper mapper)
    {
        _lotteryRepository = lotteryRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<LotteryDto>>> Handle(GetAllLotteriesQuery request, CancellationToken ct)
    {
        // 1. Получаем список доменных сущностей (IReadOnlyList<Lottery>)
        var lotteries = await _lotteryRepository.GetAllAsync(ct); 
        
        // 2. Маппим коллекцию в List<LotteryDto>. 
        // AutoMapper автоматически определит тип наследников (Bingo или KOutOfN) 
        // благодаря правилам, которые мы заложили в MappingProfile.
        var dtos = _mapper.Map<List<LotteryDto>>(lotteries);
        
        // 3. Возвращаем типизированный результат
        return Result<List<LotteryDto>>.Success(dtos); 
    }
}