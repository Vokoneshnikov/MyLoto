using FluentValidation;
using MyLoto.Application.Queries.Tickets;

namespace MyLoto.Application.Validators;

public class GetRandomNumbersQueryValidator : AbstractValidator<GetRandomNumbersQuery>
{
    public GetRandomNumbersQueryValidator()
    {
        RuleFor(x => x.DrawId)
            .GreaterThan(0).WithMessage("Идентификатор тиража (DrawId) должен быть больше 0");
    }
}