using FluentValidation;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Validators.Commands
{
    public class CreateDrawCommandValidator : AbstractValidator<CreateDrawCommand>
    {
        public CreateDrawCommandValidator()
        {
            RuleFor(x => x.LotteryId)
                .GreaterThan(0)
                .WithMessage("Id лотереи должен быть больше нуля.");

            RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Дата окончания розыгрыша должна быть в будущем.");
        }
    }
}