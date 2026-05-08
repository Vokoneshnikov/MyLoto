using FluentValidation;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Validators.Tickets
{
    public class GetUserTicketsQueryValidator : AbstractValidator<GetUserTicketsQuery>
    {
        public GetUserTicketsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId должен быть больше 0");
        }
    }
}