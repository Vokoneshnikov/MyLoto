using FluentValidation;
using MyLoto.Application.Commands.Lotteries;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Validators;

public class CreateLotteryCommandValidator : AbstractValidator<CreateLotteryCommand>
{
    public CreateLotteryCommandValidator()
    {
        // 1. Базовые правила
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название лотереи не может быть пустым.")
            .MaximumLength(100).WithMessage("Название лотереи не должно превышать 100 символов.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание лотереи не может быть пустым.")
            .MaximumLength(500).WithMessage("Описание лотереи не должно превышать 500 символов.");

        RuleFor(x => x.TicketPrice)
            .GreaterThan(0).WithMessage("Цена билета должна быть больше 0.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Тип лотереи не поддерживается.");

        RuleFor(x => x.PrizeTiers)
            .NotEmpty().WithMessage("Необходимо указать хотя бы одно правило выплат.");

        // 2. Валидация специфичных полей лотереи
        When(x => x.Type == LotteryType.K_Out_Of_N, () =>
        {
            RuleFor(x => x.K).NotNull().GreaterThan(0);
            RuleFor(x => x.N).NotNull().GreaterThan(x => x.K ?? 0)
                .WithMessage("N должно быть больше K.");
        });

        When(x => x.Type == LotteryType.Bingo, () =>
        {
            RuleFor(x => x.Rows).NotNull().GreaterThan(0);
            RuleFor(x => x.Columns).NotNull().GreaterThan(0);
            RuleFor(x => x.MaxBallValue).NotNull().GreaterThan(30);
            RuleFor(x => x.JackpotThreshold).NotNull().GreaterThan(0);
        });

        // 3. Валидация PrizeTiers с доступом к родительскому объекту (команде)
        // Используем перегрузку Must, чтобы иметь доступ и к команде (root), и к правилу (tier)
        RuleForEach(x => x.PrizeTiers).Must((command, tier) =>
        {
            if (command.Type == LotteryType.K_Out_Of_N)
            {
                // Для K из N проверяем, что условие не больше K
                return tier.RuleType == PrizeTierRuleType.MatchedNumbers && 
                       tier.ConditionValue <= (command.K ?? 0);
            }

            if (command.Type == LotteryType.Bingo)
            {
                // Для Бинго проверяем, что условие не больше MaxBallValue
                return (tier.RuleType == PrizeTierRuleType.ClosedAtBall || tier.RuleType == PrizeTierRuleType.Jackpot) && 
                       tier.ConditionValue <= (command.MaxBallValue ?? 0);
            }

            return true;
        })
        .WithMessage((command, tier) => command.Type == LotteryType.K_Out_Of_N 
            ? $"Для {command.Name} условие ({tier.ConditionValue}) не может быть больше K ({command.K})."
            : $"Для {command.Name} номер шара ({tier.ConditionValue}) не может превышать максимум ({command.MaxBallValue}).");
    }
}