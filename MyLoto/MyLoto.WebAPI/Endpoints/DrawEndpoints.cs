using MediatR;
using MyLoto.Application.Commands.Draws;
using MyLoto.Application.Queries.Draws;
using MyLoto.WebAPI.Extensions;

namespace MyLoto.WebAPI.Endpoints;

public static class DrawEndpoints
{
    public static void MapDrawEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/draws")
            .WithTags("Draws");

        // Создать новый тираж
        group.MapPost("/", async (CreateDrawCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            return result.ToProcessResult();
        });
        
        group.MapGet("/{lotteryId:long}/history", async (long lotteryId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetDrawHistoryQuery(lotteryId));
                return result.ToProcessResult();
            })
            .WithName("GetDrawHistory");
        
        // Список активных тиражей (доступных для покупки)
        group.MapGet("/active", async (ISender mediator) =>
            (await mediator.Send(new GetActiveDrawsQuery())).ToProcessResult());

        // 🔥 ДОБАВЛЕНО: Список тиражей, которые идут в эфире прямо сейчас
        group.MapGet("/live", async (ISender mediator) =>
            (await mediator.Send(new GetLiveDrawsQuery())).ToProcessResult());

        // Инфо по конкретному тиражу
        group.MapGet("/{id:long}", async (long id, ISender mediator) =>
            (await mediator.Send(new GetDrawByIdQuery(id))).ToProcessResult());
        
        group.MapPost("/{id:long}/start", async (long id, ISender mediator) =>
            {
                var result = await mediator.Send(new StartDrawCommand(id));
                return result.ToProcessResult();
            })
            .WithName("StartDraw");
        
        group.MapPost("/{drawId:long}/complete", async (long drawId, ISender mediator) =>
            {
                var result = await mediator.Send(new CompleteDrawCommand(drawId));
                return result.ToProcessResult();
            })
            .WithName("CompleteDraw");
        
        app.MapGet("api/draws/{id}/live-status", async (long id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetDrawLiveStatusQuery(id));
    
            return result.IsSuccess 
                ? Results.Ok(result.Value) 
                : Results.BadRequest(result.Error);
        });
    }
}