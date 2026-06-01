namespace MyLoto.Domain.Exceptions;

public class DuplicateTicketNumbersException : DomainException
{
    public DuplicateTicketNumbersException() 
        : base("Билет с таким набором чисел уже существует в этом тираже", "DUPLICATE_TICKET_NUMBERS")
    { }
}