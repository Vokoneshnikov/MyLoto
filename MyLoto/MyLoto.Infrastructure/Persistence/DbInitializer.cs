using MyLoto.Domain.Entities;
using MyLoto.Domain.Enums;

namespace MyLoto.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(LotoDbContext context)
    {
        if (!context.Lotteries.Any())
        {
            var classicLottery = new Lottery
            {
                Name = "Классика 6/45",
                Description = "Старая добрая лотерея. Угадай 6 чисел и стань миллионером!",
                TicketPrice = 100m,
                Type = LotteryType.K_Out_Of_N,
                K = 6,
                N = 45,
                IsPaused = false
            };

            classicLottery.PrizeTiers.Add(new PrizeTier { MatchingCondition = 2, RewardValue = 2m });
            classicLottery.PrizeTiers.Add(new PrizeTier { MatchingCondition = 3, RewardValue = 5m });

            context.Lotteries.Add(classicLottery);
        }

        // 4. Можно добавить тестового модератора (пароль пока просто строкой, потом прикрутим хеширование)
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