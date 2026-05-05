using MyLoto.Infrastructure.DependencyInjection;
using Serilog;
using MyLoto.Application;
using MyLoto.Infrastructure.Persistence; // Для сидинга

// 1. Инициализация статического логгера для раннего старта
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Приложение MyLoto запускается...");

    var builder = WebApplication.CreateBuilder(args);

    // 2. Настраиваем Serilog как основной логгер
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        // Настройка красивого вывода в консоль
        .WriteTo.Console(outputTemplate: 
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // Добавляем слои (твои существующие методы)
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddControllers();

    var app = builder.Build();

    // 3. Добавляем middleware для логирования HTTP-запросов
    app.UseSerilogRequestLogging(); 

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.MapControllers();

    // Твой блок инициализации БД (уже существующий)
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
    Log.CloseAndFlush(); // Важно для записи всех логов перед выходом
}