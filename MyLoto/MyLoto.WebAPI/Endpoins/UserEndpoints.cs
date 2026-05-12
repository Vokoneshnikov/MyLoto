using MediatR;
using MyLoto.Application.Commands.Users;
using MyLoto.Application.Queries.Tickets;
using MyLoto.Application.Queries.Users;
using MyLoto.WebAPI.Extensions;

namespace MyLoto.WebAPI.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/profile")
            .WithTags("Users")
            .RequireAuthorization();;
        
        group.MapGet("/", async (ISender mediator) =>
            {
                var result = await mediator.Send(new GetUserProfileQuery());
                return result.ToProcessResult();
            })
            .WithName("GetUserProfile");

        // Пополнение баланса
        group.MapPost("/top-up", async (decimal amount, ISender mediator) =>
        {
            var result = await mediator.Send(new TopUpBalanceCommand(amount));
            return result.ToProcessResult();
        });
        
        group.MapGet("/tickets", async (ISender mediator) =>
            {
                var query = new GetUserTicketsQuery();
                var result = await mediator.Send(query);
            
                return result.ToProcessResult();
            })
            .WithName("GetUserTickets");
        
        group.MapPut("/", async (UpdateProfileDto dto, ISender mediator) =>
        {
            var command = new UpdateProfileInfoCommand(dto.Name, dto.Surname, dto.Address);
            var result = await mediator.Send(command);
            return result.ToProcessResult();
        });
    }
}