using Typeneering.Domain.Session.Entities;

namespace Typeneering.Application.Sessions.Contracts.Responses;

public sealed record UserSessionResponse(SessionEntity Session) : AbstractSessionResponse(Session)
{
    public string Username { get; init; } = Session.User.UserName ?? string.Empty;
};
