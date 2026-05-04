namespace MyLoto.Domain.Exceptions;

// Дубликат билета (если бизнес-логика запрещает одинаковые наборы в одном тираже)
public class DuplicateTicketNumbersException : DomainException
{
    public DuplicateTicketNumbersException() 
        : base("Билет с таким набором чисел уже существует в этом тираже", "DUPLICATE_TICKET_NUMBERS")
    { }
}