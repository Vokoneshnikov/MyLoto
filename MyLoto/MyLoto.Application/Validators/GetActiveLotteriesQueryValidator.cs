using FluentValidation;
using MyLoto.Application.Queries.Lotteries;

namespace MyLoto.Application.Validators.Lotteries
{
    public class GetActiveLotteriesQueryValidator : AbstractValidator<GetActiveLotteriesQuery>
    {
        public GetActiveLotteriesQueryValidator()
        {
            // Здесь можно добавить дополнительные проверки, если они нужны
        }
    }
}