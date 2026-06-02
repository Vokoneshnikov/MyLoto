using MyLoto.Domain.Entities;

namespace MyLoto.Application;

public static class LotteryGenerator
{
    public static List<int> GenerateNumbers(Lottery lottery)
    {
        return lottery switch
        {
            KOutOfNLottery kLottery => GenerateUniqueNumbers(
                maxValue: kLottery.MaxNumber,
                count: kLottery.NumbersToChoose,
                sortResult: true),

            BingoLottery bingoLottery => GenerateUniqueNumbers(
                maxValue: bingoLottery.MaxBallValue,
                count: bingoLottery.Rows * bingoLottery.Columns,
                sortResult: false),

            _ => throw new InvalidOperationException("Неизвестный тип лотереи")
        };
    }

    private static List<int> GenerateUniqueNumbers(int maxValue, int count, bool sortResult)
    {
        if (maxValue <= 0)
        {
            throw new InvalidOperationException("Максимальное число должно быть больше 0.");
        }

        if (count <= 0)
        {
            throw new InvalidOperationException("Количество чисел должно быть больше 0.");
        }

        if (count > maxValue)
        {
            throw new InvalidOperationException(
                $"Нельзя выбрать {count} уникальных чисел из диапазона 1..{maxValue}.");
        }

        var numbers = Enumerable.Range(1, maxValue).ToList();
        Shuffle(numbers);

        var result = numbers.Take(count).ToList();

        return sortResult
            ? result.OrderBy(x => x).ToList()
            : result;
    }

    private static void Shuffle(List<int> numbers)
    {
        var random = Random.Shared;

        for (var i = numbers.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);

            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }
    }
}