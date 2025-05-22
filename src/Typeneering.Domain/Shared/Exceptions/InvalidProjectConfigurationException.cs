namespace Typeneering.Domain.Shared.Exceptions;

public class InvalidProjectConfigurationException : Exception
{
    private const string _message = "Errors have ocurred with the project's configurations";
    public InvalidProjectConfigurationException() : base(_message)
    {
    }
}
