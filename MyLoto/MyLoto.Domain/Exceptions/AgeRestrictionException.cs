namespace MyLoto.Domain.Exceptions;

// Возрастное ограничение
public class AgeRestrictionException : DomainException
{
    public AgeRestrictionException(int currentAge, int minAge) 
        : base(
            $"Доступ запрещен. Ваш возраст: {currentAge}, минимальный возраст: {minAge}", 
            "AGE_RESTRICTION",
            new { CurrentAge = currentAge, MinAge = minAge })
    { }
}