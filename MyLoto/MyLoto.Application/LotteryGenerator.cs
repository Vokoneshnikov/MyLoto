using MyLoto.Domain.Entities;

namespace MyLoto.Application;

public static class LotteryGenerator
{
    public static List<int> GenerateNumbers(Lottery lottery)
    {
        var random = new Random();
        return lottery switch
        {
            KOutOfNLottery k => Enumerable.Range(1, k.MaxNumber)
                .OrderBy(_ => random.Next()).Take(k.NumbersToChoose).OrderBy(n => n).ToList(),
            
            BingoLottery b => Enumerable.Range(1, 90)
                .OrderBy(_ => random.Next()).Take(b.Rows * b.Columns).ToList(),
            
            _ => throw new InvalidOperationException("Неизвестный тип лотереи")
        };
    }
}