using FluentValidation;
using MyLoto.Application.Queries.Draws;

namespace MyLoto.Application.Validators;

public class GetActiveDrawsQueryValidator : AbstractValidator<GetActiveDrawsQuery>
{
    public GetActiveDrawsQueryValidator()
    {
        // На данный момент правил нет
    }
}