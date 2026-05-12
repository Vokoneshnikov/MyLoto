using FluentValidation;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Application.Commands.Tickets;

namespace MyLoto.Application.Validators.Tickets;

public class GiftTicketCommandValidator : AbstractValidator<GiftTicketCommand>
{
    public GiftTicketCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.RecipientLogin)
            .NotEmpty().WithMessage("Логин получателя не может быть пустым")
            .MustAsync(async (login, ct) => await userRepository.IsLoginUniqueAsync(login, ct))
            .WithMessage("Пользователь с таким логином не существует");
    }
}