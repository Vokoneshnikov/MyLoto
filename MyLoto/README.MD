# Основная миграция:

### 1. Создание миграции

```
dotnet ef migrations add InitialCreate --project MyLoto.Infrastructure --startup-project MyLoto.WebAPI --context LotoDbContext --output-dir Migrations
```

### 2.Применение миграции

```
dotnet ef migrations add InitialCreate --project MyLoto.Infrastructure --startup-project MyLoto.WebAPI --context LotoDbContext --output-dir Migrations
```

### 3. Выполнение кода(Seeding)

Написать в MyLoto.WebAPI:
```
dotnet run
```