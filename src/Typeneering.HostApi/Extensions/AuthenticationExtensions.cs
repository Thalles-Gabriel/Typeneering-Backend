using System.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Typeneering.Domain.Shared.Exceptions;
using Typeneering.Domain.Shared;

namespace Typeneering.HostApi.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettingOptions = configuration.GetSection(JwtOptions.SectionName) ?? throw new InvalidProjectConfigurationException();
        var hashedKey = configuration.GetSection("Jwt:SecurityKey").Value ?? throw new InvalidProjectConfigurationException();
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(hashedKey));

        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtSettingOptions[nameof(JwtOptions.Issuer)] ?? throw new InvalidProjectConfigurationException();
            options.Audience = jwtSettingOptions[nameof(JwtOptions.Audience)] ?? throw new InvalidProjectConfigurationException();
            options.AccessTokenExpiration = int.Parse(jwtSettingOptions[nameof(JwtOptions.AccessTokenExpiration)]
                                                ?? throw new InvalidProjectConfigurationException());
            options.RefreshTokenExpiration = int.Parse(jwtSettingOptions[nameof(JwtOptions.RefreshTokenExpiration)]
                                                ?? throw new InvalidProjectConfigurationException());
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
        });

        var tokenParams = new TokenValidationParameters
        {
            ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = configuration.GetSection("Jwt:Audience").Value,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenParams;
            options.SaveToken = true;
        });

        return services;
    }
}
