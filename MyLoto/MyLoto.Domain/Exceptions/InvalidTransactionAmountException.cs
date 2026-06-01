namespace MyLoto.Domain.Exceptions;

public class InvalidTransactionAmountException : DomainException
{
    public InvalidTransactionAmountException(decimal amount, decimal min, decimal max) 
        : base(
            $"Сумма {amount} вне допустимого диапазона ({min} - {max})", 
            "INVALID_TRANSACTION_AMOUNT",
            new { Amount = amount, Min = min, Max = max })
    { }
}