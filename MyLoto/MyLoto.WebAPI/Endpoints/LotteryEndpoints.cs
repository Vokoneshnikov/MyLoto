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

        // Эндпоинт: Получение только активных лотерей (для обычных игроков)
        group.MapGet("/active", async (IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetActiveLotteriesQuery(), ct);
                return result.ToProcessResult();
            })
            .WithName("GetActiveLotteries");

        // НОВЫЙ ЭНДПОИНТ: Получение всех лотерей (для админ-панели)
        // Если у тебя еще нет GetAllLotteriesQuery, можешь пока использовать GetActiveLotteriesQuery 
        // или написать аналогичный запрос, вытягивающий всё без фильтра IsPaused == false
        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetAllLotteriesQuery(), ct); 
                return result.ToProcessResult();
            })
            .WithName("GetAllLotteries")
            .RequireAuthorization(policy => policy.RequireRole(UserRole.Moderator.ToString()));
        
        group.MapPost("/create", async (CreateLotteryCommand command, ISender mediator) =>
            {
                var result = await mediator.Send(command);
                return result.ToProcessResult();
            })
            .WithName("CreateLottery")
            .RequireAuthorization(policy => policy.RequireRole(UserRole.Moderator.ToString()));

        // НОВЫЙ ЭНДПОИНТ: Переключение паузы
        group.MapPost("/{id:int}/toggle-pause", async (int id, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new ToggleLotteryPauseCommand(id), ct);
                return result.ToProcessResult();
            })
            .WithName("ToggleLotteryPause")
            .RequireAuthorization(policy => policy.RequireRole(UserRole.Moderator.ToString()));
    }
}