namespace RetroGaming.Domain.Exceptions;

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.")
    {
    }
}
