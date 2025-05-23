using System.Net;
using System.Reflection;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Typeneering.Application.Base.Validators;
using Typeneering.Application.Handlers.Claims;
using Typeneering.Application.Handlers.Exceptions;
using Typeneering.Application.Handlers.Token;
using Typeneering.Application.Sessions.Services;
using Typeneering.Application.UserPreferences.Services;
using Typeneering.Application.Users.Services;
using Typeneering.Domain.Shared.Exceptions;
using Typeneering.HostApi.Constants;
using Typeneering.Infraestructure;

namespace Typeneering.HostApi.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddAppAuthentication(configuration)
                    .AddOpenApi()
                    .AddProblemDetails()
                    .AddSerilog()
                    .AddExceptionHandler<GlobalExceptionHandler>()
                    .AddSwaggerGen()
                    .AddAuthorization()
                    .AddHttpContextAccessor()
                    .AddApiRateLimiter()
                    .AddDbContext<TypeneeringDbContext>();

    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<IJWTTokenHandler, JWTTokenHandler>()
                    .AddScoped<IUserContextHandler, UserContextHandler>()
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<ISessionService, SessionService>()
                    .AddScoped<IUserPreferenceService, UserPreferenceService>()
                    .AddValidatorsFromAssemblyContaining(typeof(PagedRequestValidator<>));


    public static IServiceCollection ServicesConfiguration(this IServiceCollection services)
        => services.Configure<ForwardedHeadersOptions>(config =>
                    {
                        config.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                        config.KnownProxies.Add(IPAddress.TryParse("nginx:4000", out var address) ? address : IPAddress.Any);
                    })
                    .ConfigureHttpJsonOptions(options =>
                    {
                        options.SerializerOptions.WriteIndented = true;
                        options.SerializerOptions.IncludeFields = true;
                    });

    private static IServiceCollection AddApiRateLimiter(this IServiceCollection services)
        => services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var webSiteHost = httpContext.Request.Headers.Host.ToString();

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: webSiteHost,
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromSeconds(RateLimiterConsts.FixedWindowSeconds),
                        PermitLimit = RateLimiterConsts.FixedWindowPermitLimit,
                        QueueLimit = RateLimiterConsts.FixedWindowQueueLimit,
                        AutoReplenishment = true,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }
                );
            });

            options.AddPolicy(RateLimiterConsts.UserPolicy, httpContext =>
                 RateLimitPartition.GetTokenBucketLimiter
                 (
                    partitionKey: httpContext.User?.Identity?.Name ?? throw new InvalidUserClaimsException(),
                    factory: partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = RateLimiterConsts.TokenBucketLimit,
                        QueueLimit = RateLimiterConsts.TokenBucketQueueLimit,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(RateLimiterConsts.TokenBucketReplenishmentSeconds),
                        TokensPerPeriod = RateLimiterConsts.TokenBucketReplenishmentTokens,
                        AutoReplenishment = true,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }
                )
            );

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (rejectedContext, cancellationToken)
                => await rejectedContext.HttpContext.Response.WriteAsJsonAsync
                (
                    new ProblemDetails
                    {
                        Title = RateLimiterConsts.ErrorTitle,
                        Detail = RateLimiterConsts.ErrorDetail,
                        Instance = rejectedContext.HttpContext.Request.Path,
                        Status = StatusCodes.Status429TooManyRequests,
                        Type = nameof(RateLimiter)
                    },
                    cancellationToken
                );
        }
    );
}
