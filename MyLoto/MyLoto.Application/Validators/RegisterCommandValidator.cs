using FluentValidation;
using MyLoto.Application.Commands.Auth;

namespace MyLoto.Application.Validators.Auth;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин не может быть пустым.")
            .MinimumLength(3).WithMessage("Логин должен содержать минимум 3 символа.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Логин может содержать только латинские буквы, цифры и знак подчеркивания.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не может быть пустым.")
            .EmailAddress().WithMessage("Некорректный формат Email.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не может быть пустым.")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя не может быть пустым.")
            .MaximumLength(50).WithMessage("Имя слишком длинное.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия не может быть пустой.")
            .MaximumLength(50).WithMessage("Фамилия слишком длинная.");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18).WithMessage("Для регистрации в лотерее вам должно быть не менее 18 лет.");
    }
}