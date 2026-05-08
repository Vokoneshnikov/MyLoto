using FluentValidation;
using MyLoto.Application.Queries.Draws;

namespace MyLoto.Application.Validators.Draws
{
    public class GetActiveDrawsQueryValidator : AbstractValidator<GetActiveDrawsQuery>
    {
        public GetActiveDrawsQueryValidator()
        {
            // В этом примере нет проверки на конкретные параметры,
            // так как GetActiveDrawsQuery не имеет обязательных полей для валидации.
            // Валидация может быть добавлена, если появятся дополнительные параметры
        }
    }
}