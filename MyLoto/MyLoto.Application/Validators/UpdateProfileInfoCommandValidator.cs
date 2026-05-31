using FluentValidation;
using MyLoto.Application.Commands.Users;

namespace MyLoto.Application.Validators;

public class UpdateProfileInfoCommandValidator : AbstractValidator<UpdateProfileInfoCommand>
{
    public UpdateProfileInfoCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя не может быть пустым.")
            .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Фамилия не может быть пустой.")
            .MaximumLength(100).WithMessage("Фамилия не должна превышать 100 символов.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Адрес не должен превышать 500 символов.");
    }
}