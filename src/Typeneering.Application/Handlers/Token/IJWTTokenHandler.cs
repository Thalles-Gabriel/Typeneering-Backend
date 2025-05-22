using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Typeneering.Domain.User.Entities;

namespace Typeneering.Application.Handlers.Token;
public interface IJWTTokenHandler
{
    AccessTokenResponse GenerateAccessToken(UserEntity user);
    IReadOnlyList<Claim> GetClaims(UserEntity user);
}
