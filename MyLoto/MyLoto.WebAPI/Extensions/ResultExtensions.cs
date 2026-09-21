using Microsoft.AspNetCore.Http;
using MyLoto.Application.Common;

namespace MyLoto.WebAPI.Extensions;

public static class ResultExtensions
{
    public static IResult ToProcessResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        // Если это ошибка валидации или "не найден" — можно настраивать статус-коды
        return result.Error.Code switch
        {
            "User.NotFound" or "Draw.NotFound" or "Lottery.NotFound" => Results.NotFound(result.Error),
            _ => Results.BadRequest(result.Error)
        };
    }
}