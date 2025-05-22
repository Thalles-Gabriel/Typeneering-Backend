using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Typeneering.Application.Sessions.Services;
using Typeneering.Application.Users.Contracts.Requests;
using Typeneering.Application.Users.Services;
using Typeneering.HostApi.Constants;
using Typeneering.HostApi.Extensions;

namespace Typeneering.HostApi.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserRoutes(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/register", async ([FromBody] UserLoginRequest login, [FromServices] IUserService userService)
                    => await userService.Register(login)).AllowAnonymous();

        routeBuilder.MapPost("/login", async ([FromBody] UserLoginRequest login, [FromServices] IUserService userService)
                    => await userService.Login(login)).AllowAnonymous();

        routeBuilder.MapPost("/refresh", async ([FromBody] RefreshRequest refreshReq, [FromServices] IUserService userService)
                    => await userService.Refresh(refreshReq)).AllowAnonymous();

        routeBuilder.MapPatch("/", async ([FromBody] PatchUserRequest userRequest, [FromServices] IUserService userService, ClaimsPrincipal userClaims)
                    => await userService.Update(userRequest, userClaims))
                                        .RequireAuthorization()
                                        .RequireRateLimiting(RateLimiterConsts.UserPolicy);

        routeBuilder.MapPost("/gh-token", async ([FromBody] string token, [FromServices] IUserService userService, ClaimsPrincipal userClaims)
                    => await userService.UpdateGithubToken(token, userClaims))
                                        .RequireAuthorization()
                                        .RequireRateLimiting(RateLimiterConsts.UserPolicy);

        return routeBuilder;
    }
}
