using FluentValidation;
using MyLoto.Application.Commands.Tickets;

namespace MyLoto.Application.Validators.Commands
{
    public class BuyTicketCommandValidator : AbstractValidator<BuyTicketCommand>
    {
        public BuyTicketCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId должен быть больше нуля.");

            RuleFor(x => x.DrawId)
                .GreaterThan(0)
                .WithMessage("DrawId должен быть больше нуля.");

            RuleFor(x => x.ChosenNumbers)
                .NotEmpty()
                .WithMessage("Необходимо выбрать хотя бы одно число.")
                .Must(numbers => numbers.Distinct().Count() == numbers.Count)
                .WithMessage("Числа не должны повторяться.");

            RuleFor(x => x.ChosenNumbers)
                .Must(numbers => numbers.All(n => n >= 1 && n <= 90))
                .WithMessage("Числа должны быть в диапазоне от 1 до 90.");
        }
    }
}