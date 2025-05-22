using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Typeneering.Domain.Shared;
using Typeneering.Domain.User.Entities;

namespace Typeneering.Application.Handlers.Token;

public class JWTTokenHandler : IJWTTokenHandler
{
    private readonly JwtOptions _jwtOptions;
    public JWTTokenHandler(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
    {
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: dataExpiracao,
            signingCredentials: _jwtOptions.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public AccessTokenResponse GenerateAccessToken(UserEntity user)
    {
        var accessTokenClaims = GetClaims(user);
        var refreshTokenClaims =  GetClaims(user);

        var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
        var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

        var accessToken = CreateToken(accessTokenClaims, dataExpiracaoAccessToken);
        var refreshToken = CreateToken(refreshTokenClaims, dataExpiracaoRefreshToken);

        var value = new AccessTokenResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = dataExpiracaoAccessToken.Ticks
        };
        return value;
    }


    public IReadOnlyList<Claim> GetClaims(UserEntity user)
    {
        return
        [
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Typ, JwtBearerDefaults.AuthenticationScheme),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.AuthTime, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer),
            new(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer),
            new(JwtRegisteredClaimNames.Name, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.PreferredUsername, user.NormalizedUserName ?? string.Empty)
        ];
    }
}
