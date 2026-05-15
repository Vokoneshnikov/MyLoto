using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;
using BCrypt.Net; // Используем BCrypt напрямую

namespace MyLoto.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(LotoDbContext context)
    {
        // 1. Инициализация лотерей (без изменений, тут всё было ок)
        if (!context.Lotteries.Any())
        {
            var classicLottery = new KOutOfNLottery
            {
                Name = "Классика 6/45",
                Description = "Классическая лотерея: выберите 6 чисел из 45. Выигрыш зависит от количества совпадений.",
                TicketPrice = 100m,
                NumbersToChoose = 6,
                MaxNumber = 45,
                AccumulatedJackpot = 1_000_000m,
                TicketSalesDuration = TimeSpan.FromMinutes(45),
                DrawProcessingDuration = TimeSpan.FromMinutes(15),
                IsPaused = false
            };

            classicLottery.PrizeTiers.Add(new PrizeTier { RuleType = PrizeTierRuleType.MatchedNumbers, ConditionValue = 2, RewardMultiplier = 2m });
            classicLottery.PrizeTiers.Add(new PrizeTier { RuleType = PrizeTierRuleType.MatchedNumbers, ConditionValue = 3, RewardMultiplier = 5m });
            classicLottery.PrizeTiers.Add(new PrizeTier { RuleType = PrizeTierRuleType.MatchedNumbers, ConditionValue = 4, RewardMultiplier = 20m });
            classicLottery.PrizeTiers.Add(new PrizeTier { RuleType = PrizeTierRuleType.MatchedNumbers, ConditionValue = 5, RewardMultiplier = 100m });

            var bingoLottery = new BingoLottery
            {
                Name = "Бинго 30/90",
                Description = "Бинго-лотерея: билет содержит 30 чисел в таблице 3x10. Джекпот зависит от первых выпавших шаров.",
                TicketPrice = 100m,
                Rows = 3,
                Columns = 10,
                MaxBallValue = 90,
                JackpotThreshold = 5,
                AccumulatedJackpot = 500_000m,
                TicketSalesDuration = TimeSpan.FromMinutes(20),
                DrawProcessingDuration = TimeSpan.FromMinutes(10),
                IsPaused = false
            };

            decimal multiplier = 30m;
            for (var ballOrder = 30; ballOrder <= 90; ballOrder++)
            {
                bingoLottery.PrizeTiers.Add(new PrizeTier { RuleType = PrizeTierRuleType.ClosedAtBall, ConditionValue = ballOrder, RewardMultiplier = multiplier });
                multiplier -= 0.5m;
                if (multiplier < 0.5m) multiplier = 0.5m;
            }

            context.Lotteries.Add(classicLottery);
            context.Lotteries.Add(bingoLottery);
        }

        // 2. Инициализация пользователей через BCrypt
        if (!context.Users.Any(u => u.Role == UserRole.Moderator))
        {
            var admin = new User
            {
                Login = "moderator",
                Email = "moderator@myloto.com",
                FirstName = "Система",
                LastName = "Модератор",
                Age = 99,
                Balance = 0,
                Role = UserRole.Moderator,
                // Хешируем через BCrypt, чтобы LoginCommandHandler мог это прочитать
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
            };
            
            context.Users.Add(admin);
        }

        if (!context.Users.Any(u => u.Login == "player"))
        {
            var player = new User
            {
                Login = "player",
                Email = "player1@test.com",
                FirstName = "Иван",
                LastName = "Игроков",
                Age = 25,
                Balance = 0,
                Role = UserRole.User,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123")
            };

            context.Users.Add(player);
        }

        await context.SaveChangesAsync();
    }
}