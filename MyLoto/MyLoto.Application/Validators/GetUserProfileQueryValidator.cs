using FluentValidation;
using MyLoto.Application.Queries.Users;

namespace MyLoto.Application.Validators.Users
{
    public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
    {
        public GetUserProfileQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId должен быть больше 0");
        }
    }
}