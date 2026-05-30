using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MyLoto.Application;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Contexts;
using MyLoto.Application.Mappings;
using MyLoto.Infrastructure;
using MyLoto.Infrastructure.Auth;
using MyLoto.Infrastructure.Persistence;
using MyLoto.WebAPI.Endpoints;
using MyLoto.WebAPI.Hubs;          // Добавлено для Хаба
using MyLoto.WebAPI.Middlewares;
using MyLoto.WebAPI.Services;      // Добавлено для DrawNotificationService
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Приложение MyLoto запускается...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: 
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    builder.Services.AddAuthorization();
    builder.Services.AddEndpointsApiExplorer();
    
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyLoto API", Version = "v1" });
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Введите JWT токен в формате: Bearer {token}"
        });
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                JwtBearerDefaults.AuthenticationScheme,
                document
            )] = []
        });
    });

    // Регистрация слоев по правилам Clean Architecture
    builder.Services.AddInfrastructure(builder.Configuration); // Слой инфраструктуры (БД)
    builder.Services.AddApplication();                        // Слой логики (MediatR, Mapper)
    builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

    // --- РЕГИСТРАЦИЯ SIGNALR И СЕРВИСА УВЕДОМЛЕНИЙ ---
    builder.Services.AddSignalR();
    builder.Services.AddTransient<IDrawNotificationService, DrawNotificationService>();

    builder.Services.AddScoped<IJwtProvider, JwtProvider>();
    
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
            };
        });
    
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IUserContext, UserContext>();
    
    // --- ОБНОВЛЕННЫЙ CORS (ВКЛЮЧЕНЫ CREDENTIALS ДЛЯ SIGNALR) ---
    builder.Services.AddCors(options => {
        options.AddDefaultPolicy(policy => {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // <-- КРИТИЧНО ВАЖНО ДЛЯ WEBSOCKETS!
        });
    });
    
    var app = builder.Build();

    app.UseHangfireDashboard();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging(); 
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyLoto API v1");
            options.RoutePrefix = "swagger";
        });
    }
    app.UseHttpsRedirection();
    
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapHub<DrawHub>("/hubs/draw");
    app.MapAuthEndpoints();
    app.MapLotteryEndpoints();
    app.MapTicketEndpoints();
    app.MapDrawEndpoints();
    app.MapUserEndpoints();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<LotoDbContext>();
            await DbInitializer.SeedAsync(context);
            Log.Information("База данных успешно проверена и заполнена!");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Ошибка при инициализации базы данных!");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение неожиданно завершило работу!");
}
finally
{
    Log.CloseAndFlush();
}