using Microsoft.AspNetCore.Mvc;
using Typeneering.Application.UserPreferences.Contracts.Requests;
using Typeneering.Application.UserPreferences.Services;
using Typeneering.HostApi.Constants;

namespace Typeneering.HostApi.Endpoints;

public static class UserPreferenceEndpoints
{
    public static RouteGroupBuilder MapUserPreferenceRoutes(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapPut("/{preferenceId}", async ([FromRoute] int preferenceId, [FromBody] string value, [FromServices] IUserPreferenceService service)
                => await service.Upsert(new PostUserPreferenceRequest(preferenceId, value)));

        routeBuilder.MapGet("/", async ([FromServices] IUserPreferenceService service)
                => await service.Get());

        routeBuilder.MapDelete("/{preferenceId}", async ([FromRoute] int preferenceId, [FromServices] IUserPreferenceService service)
                => await service.Delete(preferenceId));

        return routeBuilder.RequireAuthorization().RequireRateLimiting(RateLimiterConsts.UserPolicy);;
    }
}
