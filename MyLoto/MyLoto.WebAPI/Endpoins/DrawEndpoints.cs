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
        
        // Список активных тиражей
        group.MapGet("/active", async (ISender mediator) =>
            (await mediator.Send(new GetActiveDrawsQuery())).ToProcessResult());

        // Инфо по конкретному тиражу
        group.MapGet("/{id:long}", async (long id, ISender mediator) =>
            (await mediator.Send(new GetDrawByIdQuery(id))).ToProcessResult());
        
        group.MapPost("/{id:long}/start", async (long id, ISender mediator) =>
            {
                var result = await mediator.Send(new StartDrawCommand(id));
                return result.ToProcessResult();
            })
            .WithName("StartDraw");
    }
}