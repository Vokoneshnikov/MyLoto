using FluentValidation;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Validators;

public class CreateDrawCommandValidator : AbstractValidator<CreateDrawCommand>
{
    public CreateDrawCommandValidator()
    {
        RuleFor(x => x.LotteryId)
            .GreaterThan(0)
            .WithMessage("Id лотереи должен быть больше нуля.");
    }
}