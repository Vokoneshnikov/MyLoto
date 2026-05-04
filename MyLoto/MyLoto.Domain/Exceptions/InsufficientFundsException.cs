namespace MyLoto.Domain.Exceptions;

// Недостаточно средств
public class InsufficientFundsException : DomainException
{
    public InsufficientFundsException(decimal currentBalance, decimal requiredAmount) 
        : base(
            $"Недостаточно средств. Баланс: {currentBalance}, требуется: {requiredAmount}", 
            "INSUFFICIENT_FUNDS",
            new { CurrentBalance = currentBalance, RequiredAmount = requiredAmount })
    { }
}