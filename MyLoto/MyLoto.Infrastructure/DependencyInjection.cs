using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Repositories;
using MyLoto.Infrastructure.Auth;
using MyLoto.Infrastructure.Persistence;
using MyLoto.Infrastructure.Persistence.Interceptors;
using MyLoto.Infrastructure.Persistence.Repositories; // И этот тоже

namespace MyLoto.Infrastructure;

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

        // === НОВОЕ: Регистрация репозиториев ===
        
        // Регистрируем UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<JwtProvider>();
        
        
        // Регистрируем специфичные репозитории
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IDrawRepository, DrawRepository>();
        services.AddScoped<ILotteryRepository, LotteryRepository>();

        // Если захочешь использовать базовый IRepository<T> напрямую:
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}