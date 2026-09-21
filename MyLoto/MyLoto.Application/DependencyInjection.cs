using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MyLoto.Application.Validators; // Добавь этот асинг, чтобы он увидел наш ValidationBehavior

namespace MyLoto.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // 1. Регистрируем AutoMapper
        services.AddAutoMapper(assembly);

        // 2. Регистрируем MediatR + НАШ ПАЙПЛАЙН ВАЛИДАЦИИ
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            
            // 🔥 ВЖУХ! Включаем автоматическую валидацию для КАЖДОГО запроса MediatR
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 3. Регистрируем все валидаторы из директории Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}