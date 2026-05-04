namespace MyLoto.Domain.Exceptions;

// Попытка подарить самому себе
public class SelfGiftingException : DomainException
{
    public SelfGiftingException() 
        : base("Вы не можете подарить билет самому себе", "SELF_GIFTING_NOT_ALLOWED")
    { }
}