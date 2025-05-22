using Microsoft.AspNetCore.Mvc;
using Typeneering.Application.Base.Contracts.Requests;
using Typeneering.Application.Sessions.Contracts.Requests;
using Typeneering.Application.Sessions.Services;
using Typeneering.HostApi.Constants;

namespace Typeneering.HostApi.Endpoints;

public static class SessionEndpoints
{
    public static RouteGroupBuilder MapSessionRoutes(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapGet("/{id}", async ([FromRoute] int id, [FromServices] ISessionService service)
                => await service.Get(id));

        routeBuilder.MapGet("/tes", async ([AsParameters] GetSessionRequest query, [FromServices] ISessionService service)
                => await service.GetList(query));

        routeBuilder.MapGet("/leaderboard", async ([AsParameters] LeaderboardRequest request, ISessionService service)
                => await service.GetLeaderboard(request));

        routeBuilder.MapPost("/", async ([FromBody] PostSessionRequest request, [FromServices] ISessionService service)
                => await service.Insert(request));

        return routeBuilder.RequireAuthorization().RequireRateLimiting(RateLimiterConsts.UserPolicy);
    }
}
