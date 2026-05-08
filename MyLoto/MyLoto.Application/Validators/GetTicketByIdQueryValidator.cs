using FluentValidation;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Validators.Tickets
{
    public class GetTicketByIdQueryValidator : AbstractValidator<GetTicketByIdQuery>
    {
        public GetTicketByIdQueryValidator()
        {
            RuleFor(x => x.TicketId)
                .GreaterThan(0).WithMessage("TicketId должен быть больше 0");
        }
    }
}