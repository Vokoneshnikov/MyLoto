using FluentValidation;
using MyLoto.Application.Commands.Users;

namespace MyLoto.Application.Validators;

public class DepositMoneyCommandValidator : AbstractValidator<DepositMoneyCommand>
{
    public DepositMoneyCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Сумма пополнения должна быть больше 0.");
    }
}