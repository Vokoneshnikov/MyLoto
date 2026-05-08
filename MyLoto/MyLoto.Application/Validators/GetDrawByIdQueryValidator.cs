using FluentValidation;
using MyLoto.Application.Queries.Draws;

namespace MyLoto.Application.Validators.Draws
{
    public class GetDrawByIdQueryValidator : AbstractValidator<GetDrawByIdQuery>
    {
        public GetDrawByIdQueryValidator()
        {
            RuleFor(x => x.DrawId)
                .GreaterThan(0).WithMessage("DrawId должен быть больше 0");
        }
    }
}