using Microsoft.AspNetCore.Http.HttpResults;
using Typeneering.Application.Base.Contracts.Responses;
using Typeneering.Application.UserPreferences.Contracts.Requests;
using Typeneering.Application.UserPreferences.Contracts.Responses;

namespace Typeneering.Application.UserPreferences.Services;

public interface IUserPreferenceService
{
    Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Upsert(PostUserPreferenceRequest preferenceRequest);
    Task<Results<NoContent, ProblemHttpResult>> Delete(int prefId);
    Task<Ok<ResultResponse<UserPreferenceResponse>>> Get();
}
