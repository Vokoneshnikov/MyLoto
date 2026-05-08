using FluentValidation;
using MyLoto.Application.Commands.Users;

namespace MyLoto.Application.Validators.Users
{
    public class TopUpBalanceCommandValidator : AbstractValidator<TopUpBalanceCommand>
    {
        public TopUpBalanceCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId должен быть больше 0.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Сумма пополнения должна быть больше 0.");
        }
    }
}