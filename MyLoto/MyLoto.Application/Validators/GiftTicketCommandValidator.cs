using FluentValidation;
using MyLoto.Application.Commands.Tickets;

namespace MyLoto.Application.Validators;

public class GiftTicketCommandValidator : AbstractValidator<GiftTicketCommand>
{
    public GiftTicketCommandValidator()
    {
        RuleFor(x => x.RecipientLogin)
            .NotEmpty().WithMessage("Логин получателя не может быть пустым");
            
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("Идентификатор билета должен быть больше 0");
    }
}