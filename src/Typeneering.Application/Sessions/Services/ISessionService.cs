using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Typeneering.Application.Base.Contracts.Requests;
using Typeneering.Application.Base.Contracts.Responses;
using Typeneering.Application.Sessions.Contracts.Requests;
using Typeneering.Application.Sessions.Contracts.Responses;

namespace Typeneering.Application.Sessions.Services;

public interface ISessionService
{
    Task<Results<Ok<SessionResponse>, ProblemHttpResult>> Get(int id);
    Task<Results<Ok<ResultResponse<SessionResponse>>, ProblemHttpResult>> GetList(GetSessionRequest requestQuery);
    Task<Results<Ok<ResultResponse<UserSessionResponse>>, ProblemHttpResult>> GetLeaderboard(LeaderboardRequest request);
    Task<Results<CreatedAtRoute<SessionResponse>, ProblemHttpResult>> Insert(PostSessionRequest sessionRequest);
}
