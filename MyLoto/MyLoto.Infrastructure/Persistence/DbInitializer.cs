using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(LotoDbContext context)
    {
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
                IsPaused = false
            };

            classicLottery.PrizeTiers.Add(new PrizeTier
            {
                RuleType = PrizeTierRuleType.MatchedNumbers,
                ConditionValue = 2,
                RewardMultiplier = 2m
            });

            classicLottery.PrizeTiers.Add(new PrizeTier
            {
                RuleType = PrizeTierRuleType.MatchedNumbers,
                ConditionValue = 3,
                RewardMultiplier = 5m
            });

            classicLottery.PrizeTiers.Add(new PrizeTier
            {
                RuleType = PrizeTierRuleType.MatchedNumbers,
                ConditionValue = 4,
                RewardMultiplier = 20m
            });

            classicLottery.PrizeTiers.Add(new PrizeTier
            {
                RuleType = PrizeTierRuleType.MatchedNumbers,
                ConditionValue = 5,
                RewardMultiplier = 100m
            });

            var bingoLottery = new BingoLottery
            {
                Name = "Бинго 30/90",
                Description = "Бинго-лотерея: билет содержит 30 чисел в таблице 3x10. Джекпот зависит от первых выпавших шаров, а основной выигрыш — от момента закрытия билета.",
                TicketPrice = 100m,
                Rows = 3,
                Columns = 10,
                MaxBallValue = 90,
                JackpotThreshold = 5,
                AccumulatedJackpot = 500_000m,
                IsPaused = false
            };

            // Основной розыгрыш Bingo.
            // Билет может закрыться с 30-го по 90-й шар.
            // Чем раньше закрытие, тем выше множитель.
            //
            // Пример простой шкалы:
            // 30-й шар = x30
            // 31-й шар = x29.5
            // ...
            // 88-й шар = x1.5
            // 89-й шар = x1
            // 90-й шар = x0.5

            decimal multiplier = 30m;

            for (var ballOrder = 30; ballOrder <= 90; ballOrder++)
            {
                bingoLottery.PrizeTiers.Add(new PrizeTier
                {
                    RuleType = PrizeTierRuleType.ClosedAtBall,
                    ConditionValue = ballOrder,
                    RewardMultiplier = multiplier
                });

                multiplier -= 0.5m;

                if (multiplier < 0.5m)
                {
                    multiplier = 0.5m;
                }
            }

            context.Lotteries.Add(classicLottery);
            context.Lotteries.Add(bingoLottery);
        }

        if (!context.Users.Any(u => u.Role == UserRole.Moderator))
        {
            context.Users.Add(new User
            {
                Login = "moderator",
                PasswordHash = "moderator_password_placeholder",
                Email = "moderator@myloto.com",
                FirstName = "Система",
                LastName = "Модератор",
                Age = 99,
                Balance = 0,
                Role = UserRole.Moderator
            });
        }

        if (!context.Users.Any(u => u.Login == "player1"))
        {
            context.Users.Add(new User
            {
                Login = "player1",
                PasswordHash = "player_pass",
                Email = "player1@test.com",
                FirstName = "Иван",
                LastName = "Игроков",
                Age = 25,
                Balance = 1000m,
                Role = UserRole.User
            });
        }

        await context.SaveChangesAsync();
    }
}