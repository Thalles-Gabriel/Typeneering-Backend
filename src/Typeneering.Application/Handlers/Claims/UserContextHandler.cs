
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Typeneering.Domain.Shared.Exceptions;

namespace Typeneering.Application.Handlers.Claims;

public sealed class UserContextHandler : IUserContextHandler
{
    private readonly ClaimsPrincipal _userClaim;

    public UserContextHandler(IHttpContextAccessor contextAccessor)
    {
        _userClaim = contextAccessor.HttpContext?.User ?? throw new InvalidUserClaimsException();
    }

    public Guid UserId => Guid.Parse(_userClaim.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidUserClaimsException());
    public bool IsAuthenticated => _userClaim.Identity?.IsAuthenticated ?? throw new InvalidUserClaimsException();
}
