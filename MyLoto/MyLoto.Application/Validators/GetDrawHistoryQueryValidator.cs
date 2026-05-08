using FluentValidation;
using MyLoto.Application.Queries.Draws;

namespace MyLoto.Application.Validators;

public class GetDrawHistoryQueryValidator : AbstractValidator<GetDrawHistoryQuery>
{
    public GetDrawHistoryQueryValidator()
    {
        RuleFor(x => x.LotteryId)
            .GreaterThan(0).WithMessage("LotteryId должен быть больше 0");
    }
}