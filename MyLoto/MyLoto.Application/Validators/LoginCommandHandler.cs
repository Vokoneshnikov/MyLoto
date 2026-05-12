using FluentValidation;
using MyLoto.Application.Commands.Auth;

namespace MyLoto.Application.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин не может быть пустым.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не может быть пустым.");
    }
}