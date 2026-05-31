using FluentValidation;
using MyLoto.Application.Commands.Draws;

namespace MyLoto.Application.Validators;

public class GetDrawLiveStatusQueryValidator : AbstractValidator<GetDrawLiveStatusQuery>
{
    public GetDrawLiveStatusQueryValidator()
    {
        RuleFor(x => x.DrawId)
            .GreaterThan(0).WithMessage("Идентификатор тиража (DrawId) должен быть больше 0");
    }
}