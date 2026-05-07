using MyLoto.Application;
using MyLoto.Application.Mappings;
using MyLoto.Infrastructure;
using MyLoto.Infrastructure.Persistence;
using MyLoto.WebAPI.Endpoints;
using Serilog;

// 1. Инициализация статического логгера для раннего старта
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Приложение MyLoto запускается...");

    var builder = WebApplication.CreateBuilder(args);

    // 2. Настраиваем Serilog как основной логгер приложения
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: 
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // --- РЕГИСТРАЦИЯ СЕРВИСОВ ---

    // Нужно для того, чтобы Swagger видел Minimal APIs (наши Endpoints)
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Регистрация слоев по правилам Clean Architecture
    builder.Services.AddInfrastructure(builder.Configuration); // Слой инфраструктуры (БД)
    builder.Services.AddApplication();                        // Слой логики (MediatR, Mapper)
    builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
    builder.Services.AddControllers();                         // Поддержка классических контроллеров

    var app = builder.Build();

    // --- НАСТРОЙКА MIDDLEWARE (Конвейер запросов) ---

    // Логирование каждого входящего HTTP-запроса
    app.UseSerilogRequestLogging(); 

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Маппинг классических контроллеров (если они есть)
    app.MapControllers();

    // МАППИНГ НАШИХ ЭНДПОИНТОВ (Minimal API)
    app.MapLotteryEndpoints();
    app.MapTicketEndpoints();
    app.MapDrawEndpoints();
    app.MapUserEndpoints();

    // --- ИНИЦИАЛИЗАЦИЯ БАЗЫ ДАННЫХ (SEEDING) ---
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<LotoDbContext>();
            // Вызываем наш сид данных из Дня 2
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
    Log.CloseAndFlush(); // Гарантируем запись всех логов в консоль/файл
}