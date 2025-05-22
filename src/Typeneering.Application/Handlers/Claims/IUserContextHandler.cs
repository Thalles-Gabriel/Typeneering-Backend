namespace Typeneering.Application.Handlers.Claims;

public interface IUserContextHandler
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
