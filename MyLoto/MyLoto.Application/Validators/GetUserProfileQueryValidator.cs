using FluentValidation;
using MyLoto.Application.Queries.Users;

namespace MyLoto.Application.Validators;

public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator()
    {
        // Запрос не содержит параметров для входной валидации
    }
}