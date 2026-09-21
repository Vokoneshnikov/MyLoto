using FluentValidation;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Commands.Tickets;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Validators;

public class BuyTicketCommandValidator : AbstractValidator<BuyTicketCommand>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IDrawRepository _drawRepository;

    public BuyTicketCommandValidator(ILotteryRepository lotteryRepository, IDrawRepository drawRepository)
    {
        _lotteryRepository = lotteryRepository;
        _drawRepository = drawRepository;

        RuleFor(x => x.DrawId)
            .GreaterThan(0)
            .WithMessage("DrawId должен быть больше нуля.");

        RuleFor(x => x)
            .CustomAsync(async (command, context, ct) =>
            {
                if (command.ChosenNumbers == null || !command.ChosenNumbers.Any())
                {
                    context.AddFailure("ChosenNumbers", "Необходимо выбрать числа.");
                    return;
                }

                if (command.ChosenNumbers.Distinct().Count() != command.ChosenNumbers.Count)
                {
                    context.AddFailure("ChosenNumbers", "Числа не должны повторяться.");
                }

                var draw = await _drawRepository.GetByIdAsync(command.DrawId, ct); 
                if (draw == null)
                {
                    context.AddFailure("DrawId", "Тираж не найден.");
                    return;
                }

                var lottery = draw.Lottery; 
                if (lottery == null)
                {
                    context.AddFailure("DrawId", "Не удалось загрузить данные лотереи для этого тиража.");
                    return;
                }

                int requiredK;
                int maxN;

                if (lottery is KOutOfNLottery kLottery)
                {
                    requiredK = kLottery.NumbersToChoose; 
                    maxN = kLottery.MaxNumber;      
                }
                else if (lottery is BingoLottery bLottery)
                {
                    requiredK = bLottery.Rows * bLottery.Columns;
                    maxN = bLottery.MaxBallValue;
                }
                else
                {
                    context.AddFailure("Lottery", "Неизвестный тип лотереи.");
                    return;
                }

                if (command.ChosenNumbers.Count != requiredK)
                {
                    context.AddFailure("ChosenNumbers", $"Вы должны выбрать ровно {requiredK} чисел.");
                }

                if (command.ChosenNumbers.Any(n => n < 1 || n > maxN))
                {
                    context.AddFailure("ChosenNumbers", $"Числа должны быть в диапазоне от 1 до {maxN}.");
                }
            });
    }
}