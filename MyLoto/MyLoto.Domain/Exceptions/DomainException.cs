namespace MyLoto.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorCode { get; }
    public object? ErrorData { get; }

    protected DomainException(string message, string errorCode, object? errorData = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorData = errorData;
    }
}