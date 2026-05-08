using FluentValidation;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Validators.Commands
{
    public class StartDrawCommandValidator : AbstractValidator<StartDrawCommand>
    {
        public StartDrawCommandValidator()
        {
            RuleFor(x => x.DrawId)
                .GreaterThan(0)
                .WithMessage("Id тиража должен быть больше нуля.");
        }
    }
}