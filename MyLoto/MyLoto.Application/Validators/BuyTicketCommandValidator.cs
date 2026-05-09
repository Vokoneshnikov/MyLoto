using FluentValidation;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Commands.Tickets;
using MyLoto.Domain.Entities;

namespace MyLoto.Application.Validators.Commands;

public class BuyTicketCommandValidator : AbstractValidator<BuyTicketCommand>
{
    private readonly ILotteryRepository _lotteryRepository;
    private readonly IDrawRepository _drawRepository;

    public BuyTicketCommandValidator(ILotteryRepository lotteryRepository, IDrawRepository drawRepository)
    {
        _lotteryRepository = lotteryRepository;
        _drawRepository = drawRepository;

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId должен быть больше нуля.");

        RuleFor(x => x.DrawId)
            .GreaterThan(0)
            .WithMessage("DrawId должен быть больше нуля.");

        // Основная цепочка валидации чисел
        RuleFor(x => x)
            .CustomAsync(async (command, context, ct) =>
            {
                // Проверка на наличие выбранных чисел
                if (command.ChosenNumbers == null || !command.ChosenNumbers.Any())
                {
                    context.AddFailure("ChosenNumbers", "Необходимо выбрать числа.");
                    return;
                }

                // 1. Проверка на уникальность
                if (command.ChosenNumbers.Distinct().Count() != command.ChosenNumbers.Count)
                {
                    context.AddFailure("ChosenNumbers", "Числа не должны повторяться.");
                }

                // 2. Ищем тираж вместе с лотереей (убедись, что GetByIdAsync делает .Include(d => d.Lottery))
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

                // 3. Определение параметров валидации в зависимости от типа лотереи
                int requiredK;
                int maxN;

                if (lottery is KOutOfNLottery kLottery)
                {
                    // Используем свойства KOutOfNLottery
                    requiredK = kLottery.NumbersToChoose; // или NumbersToChoose, если ты его так назвал
                    maxN = kLottery.MaxNumber;      // или MaxNumber
                }
                else if (lottery is BingoLottery bLottery)
                {
                    // Для Бинго требуется заполнение всей сетки
                    requiredK = bLottery.Rows * bLottery.Columns;
                    maxN = bLottery.MaxBallValue;
                }
                else
                {
                    context.AddFailure("Lottery", "Неизвестный тип лотереи.");
                    return;
                }

                // 4. Валидация количества выбранных чисел
                if (command.ChosenNumbers.Count != requiredK)
                {
                    context.AddFailure("ChosenNumbers", $"Вы должны выбрать ровно {requiredK} чисел.");
                }

                // 5. Динамическая валидация диапазона
                if (command.ChosenNumbers.Any(n => n < 1 || n > maxN))
                {
                    context.AddFailure("ChosenNumbers", $"Числа должны быть в диапазоне от 1 до {maxN}.");
                }
            });
    }
}