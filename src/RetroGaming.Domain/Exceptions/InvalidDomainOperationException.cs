namespace RetroGaming.Domain.Exceptions;

public class InvalidDomainOperationException : DomainException
{
    public InvalidDomainOperationException(string message) : base(message)
    {
    }

    public InvalidDomainOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
