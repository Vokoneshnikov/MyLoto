using MediatR;
using MyLoto.Application.Commands.Tickets;
using MyLoto.Application.Queries.Tickets;
using MyLoto.WebAPI.Extensions; // Теперь это заработает!

namespace MyLoto.WebAPI.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/tickets")
            .WithTags("Tickets");

        group.MapPost("/buy", async (BuyTicketCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            return result.ToProcessResult();
        });
        
        group.MapPost("/gift", async (GiftTicketCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            return result.ToProcessResult();
        });
        
        // Детальная информация о билете
        group.MapGet("/{ticketId:long}", async (long ticketId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetTicketByIdQuery(ticketId));
                return result.ToProcessResult();
            })
            .WithName("GetTicketById");
        // Внутри MapTicketEndpoints
        group.MapGet("/{drawId:long}/random", async (long drawId, ISender mediator) =>
        {
            var result = await mediator.Send(new GetRandomNumbersQuery(drawId));
            return result.ToProcessResult();
        });
    }
}