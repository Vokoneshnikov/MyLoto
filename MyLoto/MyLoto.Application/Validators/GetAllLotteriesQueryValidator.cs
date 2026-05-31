using FluentValidation;
using MyLoto.Application.Queries.Lotteries;

namespace MyLoto.Application.Validators;

public class GetAllLotteriesQueryValidator : AbstractValidator<GetAllLotteriesQuery>
{
    public GetAllLotteriesQueryValidator()
    {
        // Сейчас у запроса нет параметров. 
        // Сюда можно будет добавить правила, если появится пагинация или поиск.
    }
}