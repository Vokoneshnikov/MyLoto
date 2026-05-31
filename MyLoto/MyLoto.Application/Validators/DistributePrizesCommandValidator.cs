using FluentValidation;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Validators;

public class DistributePrizesCommandValidator : AbstractValidator<DistributePrizesCommand>
{
    public DistributePrizesCommandValidator()
    {
        RuleFor(x => x.DrawId)
            .GreaterThan(0).WithMessage("Идентификатор тиража (DrawId) для распределения призов должен быть больше 0");
    }
}