using FluentValidation;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Validators;

public class CheckPrizesCommandValidator : AbstractValidator<CheckPrizesCommand>
{
    public CheckPrizesCommandValidator()
    {
        RuleFor(x => x.DrawId)
            .GreaterThan(0).WithMessage("Идентификатор тиража (DrawId) для проверки билетов должен быть больше 0");
    }
}