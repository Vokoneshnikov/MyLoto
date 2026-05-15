using MediatR;
using MyLoto.Application.Commands.Lotteries;
using MyLoto.Application.Queries.Lotteries;
using MyLoto.Domain.Enums;
using MyLoto.WebAPI.Extensions;

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
        
        group.MapPost("/create", async (CreateLotteryCommand command, ISender mediator) =>
            {
                var result = await mediator.Send(command);
                return result.ToProcessResult();
            })
            .WithName("CreateLottery")
            .RequireAuthorization(policy => policy.RequireRole(UserRole.Moderator.ToString()));
    }
}