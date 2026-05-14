using MediatR;
using MyLoto.Application.Commands.Auth;
using MyLoto.Application.Common;

namespace MyLoto.WebAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", async (LoginCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            
            return result.IsSuccess 
                ? Results.Ok(result) 
                : Results.BadRequest(result);
        });

        group.MapPost("/register", async (RegisterCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            
            return result.IsSuccess 
                ? Results.Ok(result) 
                : Results.BadRequest(result);
        });
    }
}