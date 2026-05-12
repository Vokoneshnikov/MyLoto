using System.Net;
using System.Text.Json;
using MyLoto.Application.Common;
using MyLoto.Domain.Exceptions; // Твой namespace с исключениями

namespace MyLoto.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        
        // Логируем всё как Error, но для DomainException можно снизить уровень до Warning, 
        // так как это "ожидаемые" нарушения бизнес-правил.
        if (exception is DomainException)
            _logger.LogWarning("Бизнес-правило нарушено: {Message}. TraceId: {TraceId}", exception.Message, traceId);
        else
            _logger.LogError(exception, "Критическая ошибка сервера. TraceId: {TraceId}", traceId);

        context.Response.ContentType = "application/json";

        // Маппинг исключений на статус-коды и объекты Error
        var (statusCode, error, details) = exception switch
        {
            // Наш главный герой — доменное исключение
            DomainException domainEx => (
                HttpStatusCode.BadRequest, 
                new Error(domainEx.ErrorCode, domainEx.Message),
                domainEx.ErrorData // Дополнительные данные (текущий баланс, возраст и т.д.)
            ),

            // Стандартные исключения C#
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized, 
                new Error("AUTH_REQUIRED", "Необходима авторизация"),
                null
            ),

            // Все остальное, что мы не предусмотрели
            _ => (
                HttpStatusCode.InternalServerError, 
                new Error("SERVER_ERROR", "Произошла непредвиденная ошибка на стороне сервера"),
                null
            )
        };

        context.Response.StatusCode = (int)statusCode;

        // Формируем итоговый JSON
        var response = new
        {
            IsSuccess = false,
            Error = error,
            Data = details, // Пробрасываем ErrorData, чтобы фронт мог сказать: "Вам не хватает 500р"
            TraceId = traceId
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}