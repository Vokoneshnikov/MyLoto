using MediatR;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Common;
using FluentValidation;
using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Commands.Lotteries;

public class CreateLotteryCommandHandler : IRequestHandler<CreateLotteryCommand, Result<long>>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateLotteryCommand> _validator;

    public CreateLotteryCommandHandler(
        ILotteryRepository lotteryRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateLotteryCommand> validator)
    {
        _lotteryRepository = lotteryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<long>> Handle(CreateLotteryCommand request, CancellationToken ct)
    {
        // Валидация данных с использованием FluentValidation
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();
            return Result<long>.Failure(new Error(firstError.PropertyName, firstError.ErrorMessage)); 
        }

        Lottery lottery;

        if (request.Type == LotteryType.K_Out_Of_N)
        {
            if (!request.K.HasValue || !request.N.HasValue)
            {
                return Result<long>.Failure(new Error("Lottery.InvalidConfig", "K и N должны быть указаны для лотереи типа K из N"));
            }
            
            lottery = new KOutOfNLottery
            {
                Name = request.Name,
                Description = request.Description,
                TicketPrice = request.TicketPrice,
                Type = LotteryType.K_Out_Of_N,
                NumbersToChoose = request.K.Value,
                AccumulatedJackpot = request.JackpotValue,
                MaxNumber = request.N.Value,
                IsPaused = request.IsPaused
            };
        }
        else if (request.Type == LotteryType.Bingo)
        {
            if (!request.Rows.HasValue || !request.Columns.HasValue || 
                !request.MaxBallValue.HasValue || !request.JackpotThreshold.HasValue)
            {
                return Result<long>.Failure(new Error("Lottery.InvalidConfig", "Для лотереи типа Bingo должны быть указаны Rows, Columns, MaxBallValue и JackpotThreshold"));
            }
            lottery = new BingoLottery
            {
                Name = request.Name,
                Description = request.Description,
                TicketPrice = request.TicketPrice,
                Type = LotteryType.Bingo,
                AccumulatedJackpot = request.JackpotValue,
                Rows = request.Rows.Value,
                Columns = request.Columns.Value,
                MaxBallValue = request.MaxBallValue.Value,
                JackpotThreshold = request.JackpotThreshold.Value,
                IsPaused = request.IsPaused
            };
        }
        else
        {
            return Result<long>.Failure(new Error("Lottery.InvalidType", "Тип лотереи не поддерживается"));
        }

        await _lotteryRepository.AddAsync(lottery, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<long>.Success(lottery.Id);
    }
}