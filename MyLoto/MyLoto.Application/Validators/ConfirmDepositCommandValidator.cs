using FluentValidation;
using MyLoto.Application.Commands.Users;

namespace MyLoto.Application.Validators;

public class ConfirmDepositCommandValidator : AbstractValidator<ConfirmDepositCommand>
{
    public ConfirmDepositCommandValidator()
    {
        RuleFor(x => x.JsonPayload)
            .NotEmpty().WithMessage("Тело запроса (JsonPayload) вебхука не может быть пустым.");

        RuleFor(x => x.Signature)
            .NotEmpty().WithMessage("Заголовок сигнатуры (Stripe-Signature) отсутствует или пуст.");
    }
}