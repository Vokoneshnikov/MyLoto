namespace MyLoto.Domain.Exceptions;

// Попытка купить билет в начавшемся тираже
public class DrawRegistrationClosedException : DomainException
{
    public DrawRegistrationClosedException(long drawId, string status) 
        : base(
            $"Регистрация на тираж {drawId} закрыта. Текущий статус: {status}", 
            "DRAW_REGISTRATION_CLOSED",
            new { DrawId = drawId, CurrentStatus = status })
    { }
}