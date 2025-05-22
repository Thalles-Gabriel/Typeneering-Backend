namespace Typeneering.Domain.Shared.Exceptions;

public class InvalidUserClaimsException : Exception
{
    private const string _message = "The claims provided are invalid";

    public InvalidUserClaimsException() : base(_message)
    {
    }

    public InvalidUserClaimsException(string? message) : base(message)
    {
    }

    public InvalidUserClaimsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
