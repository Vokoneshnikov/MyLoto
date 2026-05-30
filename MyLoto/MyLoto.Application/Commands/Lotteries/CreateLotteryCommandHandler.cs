using FluentValidation;
using Hangfire;
using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using MyLoto.Application.Common.BackgroundJobs;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Lotteries;

public class CreateLotteryCommandHandler : IRequestHandler<CreateLotteryCommand, Result<long>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateLotteryCommand> _validator;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public CreateLotteryCommandHandler(
        ILotteryRepository lotteryRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateLotteryCommand> validator,
        IBackgroundJobClient backgroundJobClient)
    {
        _lotteryRepository = lotteryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Result<long>> Handle(CreateLotteryCommand request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(new Error("Validation.Error", validationResult.Errors.First().ErrorMessage)); 
        }

        Lottery lottery = request.Type switch
        {
            LotteryType.K_Out_Of_N => CreateKOutOfN(request),
            LotteryType.Bingo => CreateBingo(request),
            _ => throw new NotImplementedException()
        };

        // Мапим DTO на сущности PrizeTier
        lottery.PrizeTiers = request.PrizeTiers.Select(dto => new PrizeTier
        {
            RuleType = dto.RuleType,
            ConditionValue = dto.ConditionValue,
            RewardMultiplier = dto.RewardMultiplier
        }).ToList();

        await _lotteryRepository.AddAsync(lottery, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // --- ИНТЕГРАЦИЯ С HANGFIRE ---
        // Если лотерея создается не на паузе, запускаем цикл создания тиражей
        if (!lottery.IsPaused)
        {
            _backgroundJobClient.Enqueue<DrawJobsManager>(x => x.TriggerCreateDraw(lottery.Id));
        }
        
        return Result<long>.Success(lottery.Id);
    }

    private KOutOfNLottery CreateKOutOfN(CreateLotteryCommand r) => new()
    {
        Name = r.Name,
        Description = r.Description,
        TicketPrice = r.TicketPrice,
        TicketSalesDuration = r.TicketSalesDuration,
        DrawProcessingDuration = r.DrawProcessingDuration,
        NumbersToChoose = r.K!.Value,
        MaxNumber = r.N!.Value,
        AccumulatedJackpot = r.JackpotValue,
        IsPaused = r.IsPaused
    };

    private BingoLottery CreateBingo(CreateLotteryCommand r) => new()
    {
        Name = r.Name,
        Description = r.Description,
        TicketPrice = r.TicketPrice,
        TicketSalesDuration = r.TicketSalesDuration,
        DrawProcessingDuration = r.DrawProcessingDuration,
        Rows = r.Rows!.Value,
        Columns = r.Columns!.Value,
        MaxBallValue = r.MaxBallValue!.Value,
        JackpotThreshold = r.JackpotThreshold!.Value,
        AccumulatedJackpot = r.JackpotValue,
        IsPaused = r.IsPaused
    };
}