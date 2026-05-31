using System.Net;
using System.Text.Json;
using MyLoto.Application.Common;
using MyLoto.Domain.Exceptions; // Твой namespace с исключениями
using FluentValidation; // Добавь этот using в самый верх файла!

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
    
        if (exception is DomainException)
            _logger.LogWarning("Бизнес-правило нарушено: {Message}. TraceId: {TraceId}", exception.Message, traceId);
        // Ошибки валидации — это тоже "ожидаемый" плохой ввод пользователя, логируем как Warning, чтобы не спамить в Error-логи
        else if (exception is ValidationException)
            _logger.LogWarning("Ошибка валидации входящих данных. TraceId: {TraceId}", traceId);
        else
            _logger.LogError(exception, "Критическая ошибка сервера. TraceId: {TraceId}", traceId);

        context.Response.ContentType = "application/json";

        // Маппинг исключений на статус-коды и объекты Error
        var (statusCode, error, details) = exception switch
        {
            // 🔥 НАШ НОВЫЙ ГЕРОЙ — Ошибка валидации FluentValidation
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new Error("VALIDATION_ERROR", "Переданные данные не прошли проверку."),
                // Группируем ошибки по имени поля и кладем в Data: { "name": ["Длина превышена"], "ticketPrice": ["Должна быть > 0"] }
                (object)validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => JsonNamingPolicy.CamelCase.ConvertName(g.Key), // Переводим имя поля в camelCase для фронта
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ),

            // Твой доменный обработчик
            DomainException domainEx => (
                HttpStatusCode.BadRequest, 
                new Error(domainEx.ErrorCode, domainEx.Message),
                domainEx.ErrorData 
            ),

            // Авторизация
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized, 
                new Error("AUTH_REQUIRED", "Необходима авторизация"),
                null
            ),

            // Все остальное
            _ => (
                HttpStatusCode.InternalServerError, 
                new Error("SERVER_ERROR", "Произошла непредвиденная ошибка на стороне сервера"),
                null
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            IsSuccess = false,
            Error = error,
            Data = details, // Здесь фронт найдет карту ошибок по полям!
            TraceId = traceId
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}