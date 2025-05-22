using Microsoft.IdentityModel.Tokens;
namespace Typeneering.Domain.Shared;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public SigningCredentials SigningCredentials { get; set; }
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpiration { get; set; }
}

