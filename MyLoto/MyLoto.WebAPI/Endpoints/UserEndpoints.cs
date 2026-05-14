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

        group.MapPost("/deposit", async (DepositMoneyDto request, ISender mediator) =>
        {
            var result = await mediator.Send(new DepositMoneyCommand(
                request.Amount, 
                request.SuccessUrl, 
                request.CancelUrl));

            return result.ToProcessResult();
        });
        group.MapPost("/stripe", async (HttpRequest request, ISender mediator) =>
        {
            request.EnableBuffering();
            Console.WriteLine("!!! ВЕБХУК ВЫЗВАН !!!");
            // Читаем JSON из тела запроса
            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();

            request.Body.Position = 0;
            
            // Забираем заголовок подписи
            var signature = request.Headers["Stripe-Signature"].ToString();

            // Отправляем всё в обработчик
            var result = await mediator.Send(new ConfirmDepositCommand(json, signature));

            if (!result.IsSuccess)
            {
                // Выводим конкретную ошибку из Handler в консоль сервера
                Console.WriteLine($"!!! ОШИБКА В HANDLER: {result.Error} !!!");
            }
            
            // Webhook должен возвращать 200 OK, даже если это не "наш" тип события
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
        })
        .AllowAnonymous()       // 1. Пропускает запрос без JWT/Cookie
        .DisableAntiforgery();;
        
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