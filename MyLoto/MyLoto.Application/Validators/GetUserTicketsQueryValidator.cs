using FluentValidation;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Validators
{
    public class GetUserTicketsQueryValidator : AbstractValidator<GetUserTicketsQuery>
    {
        public GetUserTicketsQueryValidator()
        {
            // Если билет еще активный (IsArchive == false), то фильтр IsWon не имеет смысла, 
            // так как тираж еще не проверен и выигрышей быть не может.
            RuleFor(x => x.IsWon)
                .Null()
                .When(x => !x.IsArchive)
                .WithMessage("Фильтрация по выигрышу доступна только для архивных билетов.");
        }
    }
}