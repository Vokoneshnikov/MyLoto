using FluentValidation;
using MyLoto.Application.Queries.Draws;

namespace MyLoto.Application.Validators;

public class GetLiveDrawsQueryValidator : AbstractValidator<GetLiveDrawsQuery>
{
    public GetLiveDrawsQueryValidator()
    {
        // На данный момент у запроса нет параметров для проверки
    }
}