using FluentValidation;
using MyLoto.Application.Commands.Lotteries;
using MyLoto.Domain.Enums;

namespace MyLoto.Application.Validators;

public class CreateLotteryCommandValidator : AbstractValidator<CreateLotteryCommand>
{
    public CreateLotteryCommandValidator()
    {
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

        // Для K_Out_Of_N
        RuleFor(x => x.K)
            .GreaterThan(0).When(x => x.Type == LotteryType.K_Out_Of_N).WithMessage("Количество чисел (K) должно быть больше 0.");

        RuleFor(x => x.N)
            .GreaterThan(0).When(x => x.Type == LotteryType.K_Out_Of_N).WithMessage("Максимальное количество чисел (N) должно быть больше 0.");

        // Для Bingo
        RuleFor(x => x.Rows)
            .GreaterThan(0).When(x => x.Type == LotteryType.Bingo).WithMessage("Количество строк в бинго должно быть больше 0.");

        RuleFor(x => x.Columns)
            .GreaterThan(0).When(x => x.Type == LotteryType.Bingo).WithMessage("Количество колонок в бинго должно быть больше 0.");

        RuleFor(x => x.MaxBallValue)
            .GreaterThan(0).When(x => x.Type == LotteryType.Bingo).WithMessage("Максимальное значение шара в бинго должно быть больше 0.");

        RuleFor(x => x.JackpotThreshold)
            .GreaterThan(0).When(x => x.Type == LotteryType.Bingo).WithMessage("Порог джекпота в бинго должен быть больше 0.");
    }
}