# Основная миграция:

### 1. Создание миграции

```
dotnet ef migrations add InitialCreate --project MyLoto.Infrastructure --startup-project MyLoto.WebAPI --context LotoDbContext --output-dir Migrations
```

### 2.Применение миграции

```
dotnet ef database update InitialCreate --project MyLoto.Infrastructure --startup-project MyLoto.WebAPI --context LotoDbContext
```

### 3. Выполнение кода(Seeding)

Написать в MyLoto.WebAPI:
```
dotnet run
```




# Stripe CLI:

'''
stripe listen --forward-to https://127.0.0.1:7162/api/profile/stripe
'''