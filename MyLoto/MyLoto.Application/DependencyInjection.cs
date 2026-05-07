using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MyLoto.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Получаем текущую сборку (проект MyLoto.Application)
        var assembly = typeof(DependencyInjection).Assembly;

        // 1. Регистрируем AutoMapper (он сам найдет MappingProfile)
        services.AddAutoMapper(assembly);

        // 2. Регистрируем MediatR (он сам найдет все классы IRequestHandler)
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));

        return services;
    }
}