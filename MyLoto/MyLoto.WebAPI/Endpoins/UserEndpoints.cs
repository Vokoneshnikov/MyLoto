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
        var group = app.MapGroup("api/users")
            .WithTags("Users");
        
        group.MapGet("/{userId:long}", async (long userId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetUserProfileQuery(userId));
                return result.ToProcessResult();
            })
            .WithName("GetUserProfile");

        // Пополнение баланса
        group.MapPost("/{userId:long}/top-up", async (long userId, decimal amount, ISender mediator) =>
        {
            var result = await mediator.Send(new TopUpBalanceCommand(userId, amount));
            return result.ToProcessResult();
        });
        
        group.MapGet("/{userId:long}/tickets", async (long userId, ISender mediator) =>
            {
                var query = new GetUserTicketsQuery(userId);
                var result = await mediator.Send(query);
            
                return result.ToProcessResult();
            })
            .WithName("GetUserTickets");
        
        group.MapPut("/{userId:long}/profile", async (long userId, UpdateProfileDto dto, ISender mediator) =>
        {
            var command = new UpdateProfileInfoCommand(userId, dto.Name, dto.Surname, dto.Address);
            var result = await mediator.Send(command);
            return result.ToProcessResult();
        });
    }
}