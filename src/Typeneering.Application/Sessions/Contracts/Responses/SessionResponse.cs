using Typeneering.Domain.Session.Entities;

namespace Typeneering.Application.Sessions.Contracts.Responses;

public sealed record SessionResponse(SessionEntity Session) : AbstractSessionResponse(Session);
