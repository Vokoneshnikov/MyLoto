using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLoto.Infrastructure.Persistence;
using MyLoto.Infrastructure.Persistence.Interceptors;

namespace MyLoto.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Регистрируем интерцептор
        services.AddSingleton<UpdateAuditableInterceptor>();

        // 2. Настраиваем DbContext
        services.AddDbContext<LotoDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<UpdateAuditableInterceptor>();
            
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(interceptor);
        });

        return services;
    }
}