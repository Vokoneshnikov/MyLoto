namespace MyLoto.Domain.Exceptions;

public class InvalidTicketNumbersException : DomainException
{
    public InvalidTicketNumbersException(string reason, object details) 
        : base(
            $"Некорректный набор чисел: {reason}", 
            "INVALID_TICKET_NUMBERS",
            details)
    { }
}