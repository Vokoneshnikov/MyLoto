using FluentValidation;
using MyLoto.Application.Queries.Lotteries;

namespace MyLoto.Application.Validators;

public class GetActiveLotteriesQueryValidator : AbstractValidator<GetActiveLotteriesQuery>
{
    public GetActiveLotteriesQueryValidator()
    {
        // Дополнительные проверки не требуются
    }
}