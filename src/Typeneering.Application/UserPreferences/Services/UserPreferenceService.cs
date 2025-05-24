using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Typeneering.Application.Base.Constants;
using Typeneering.Application.Base.Contracts.Responses;
using Typeneering.Application.Handlers.Claims;
using Typeneering.Application.UserPreferences.Contracts.Requests;
using Typeneering.Application.UserPreferences.Contracts.Responses;
using Typeneering.Domain.Preference.Entities;
using Typeneering.Infraestructure;

namespace Typeneering.Application.UserPreferences.Services;

public class UserPreferenceService : IUserPreferenceService
{
    private readonly IUserContextHandler _userContext;
    private readonly TypeneeringDbContext _dbContext;

    public UserPreferenceService(TypeneeringDbContext dbContext, IUserContextHandler userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Upsert(PostUserPreferenceRequest preferenceRequest)
    {
        var preference = await _dbContext.Preferences.FindAsync(preferenceRequest.PreferenceId);

        if (preference is null)
            return TypedResults.Problem(
                    title: "Preference doesn't exist",
                    detail: "Selected preference does not exist",
                    type: "ErrorPreferenceNull",
                    statusCode: StatusCodes.Status400BadRequest
                    );

        var userPref = await _dbContext.UserPreferences.Where(upref => upref.Preference == preference
                                                                        && upref.UserId == _userContext.UserId)
                                                        .FirstOrDefaultAsync();

        if (userPref is null)
        {
            await _dbContext.UserPreferences.AddAsync(new UserPreferenceEntity
            {
                UserId = _userContext.UserId,
                PreferenceId = preference.Id,
                Value = preferenceRequest.Value
            });
        }
        else
        {
            userPref.Value = preferenceRequest.Value;
            userPref.UpdatedAt = DateTimeOffset.UtcNow;
        }

        if (await _dbContext.SaveChangesAsync() < 1)
            return TypedResults.Problem(
                    title: "Error changing user preference",
                    detail: "User preference was not updated",
                    type: "ErrorUpsertUserPreference",
                    statusCode: StatusCodes.Status422UnprocessableEntity
                    );

        return TypedResults.Ok(new ResultResponse(
                    Title: "User preference was changed successfully",
                    Message: "The selected preference was successfully update for the user"
                    ));

    }

    public async Task<Results<NoContent, ProblemHttpResult>> Delete(int prefId)
    {
        var rowsAffected = await _dbContext.UserPreferences.Where(upref => upref.UserId == _userContext.UserId
                                                                    && upref.PreferenceId == prefId)
                                                            .ExecuteDeleteAsync();

        if (rowsAffected < 1)
            return TypedResults.Problem(
                    title: "Preference doesn't exist",
                    detail: "Selected preference does not exist",
                    type: "ErrorPreferenceNull",
                    statusCode: StatusCodes.Status400BadRequest
                    );

        return TypedResults.NoContent();
    }

    public async Task<Ok<ResultResponse<UserPreferenceResponse>>> Get()
    {
        var userPreferences = await _dbContext.UserPreferences.Where(upref => upref.UserId == _userContext.UserId)
                                                                .Include(upref => upref.Preference)
                                                                .Select(upref => new UserPreferenceResponse(upref.Preference.Name, upref.Value))
                                                                .ToListAsync();

        return TypedResults.Ok(new ResultResponse<UserPreferenceResponse>(
                    Title: BaseConsts.Results.RetrievedTitle,
                    Message: BaseConsts.Results.ItemsRetrieved(userPreferences.Count),
                    userPreferences,
                    userPreferences.Count
                    ));
    }
}
