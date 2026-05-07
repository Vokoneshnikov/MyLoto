using MediatR;
using MyLoto.Application.Queries.Lotteries;
using MyLoto.WebAPI.Extensions; // Подключаем наши расширения

namespace MyLoto.WebAPI.Endpoints;

public static class LotteryEndpoints
{
    public static void MapLotteryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/lotteries")
            .WithTags("Lotteries");

        // Эндпоинт: Получение активных лотерей
        group.MapGet("/active", async (IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetActiveLotteriesQuery(), ct);
            
                // Теперь просто вызываем наш метод расширения
                return result.ToProcessResult();
            })
            .WithName("GetActiveLotteries");
    }
}