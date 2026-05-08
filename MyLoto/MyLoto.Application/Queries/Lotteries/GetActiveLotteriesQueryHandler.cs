using AutoMapper;
using FluentValidation;
using MediatR;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Queries.Lotteries;

namespace MyLoto.Application.Queries.Lotteries;

public class GetActiveLotteriesQueryHandler 
    : IRequestHandler<GetActiveLotteriesQuery, Result<IReadOnlyList<LotteryDto>>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetActiveLotteriesQuery> _validator; // Вставлен валидатор

    public GetActiveLotteriesQueryHandler(
        ILotteryRepository lotteryRepository, 
        IMapper mapper, 
        IValidator<GetActiveLotteriesQuery> validator)
    {
        _lotteryRepository = lotteryRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<LotteryDto>>> Handle(
        GetActiveLotteriesQuery request, 
        CancellationToken cancellationToken)
    {
        // Валидация запроса
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<IReadOnlyList<LotteryDto>>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage));
        }

        // Получаем активные лотереи
        var lotteries = await _lotteryRepository.GetActiveLotteriesAsync(cancellationToken);

        // Маппим сущности Domain в DTO
        var dtos = _mapper.Map<IReadOnlyList<LotteryDto>>(lotteries);

        // Возвращаем успешный результат
        return Result<IReadOnlyList<LotteryDto>>.Success(dtos);
    }
}