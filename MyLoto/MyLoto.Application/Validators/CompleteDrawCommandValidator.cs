using FluentValidation;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Validators;

public class CompleteDrawCommandValidator : AbstractValidator<CompleteDrawCommand>
{
    public CompleteDrawCommandValidator()
    {
        RuleFor(x => x.DrawId)
            .GreaterThan(0).WithMessage("Идентификатор тиража (DrawId) для его финализации должен быть больше 0");
    }
}