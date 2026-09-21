using FluentValidation;
using MyLoto.Application.Commands.Lotteries;

namespace MyLoto.Application.Validators;

public class ToggleLotteryPauseCommandValidator : AbstractValidator<ToggleLotteryPauseCommand>
{
    public ToggleLotteryPauseCommandValidator()
    {
        RuleFor(x => x.LotteryId)
            .GreaterThan(0).WithMessage("Идентификатор лотереи (LotteryId) должен быть больше 0");
    }
}