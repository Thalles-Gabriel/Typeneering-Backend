using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Typeneering.Application.Base.Contracts.Responses;
using Typeneering.Application.Users.Contracts.Requests;
using Typeneering.Application.Users.Contracts.Responses;

namespace Typeneering.Application.Users.Services;

public interface IUserService
{
    Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Register(UserRegisterRequest login);
    Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> Login(UserLoginRequest login);
    Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> Refresh(RefreshRequest refreshRequest);
    Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Update(PatchUserRequest userRequest, ClaimsPrincipal userClaims);
    Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Delete(ClaimsPrincipal userClaims);
    Task<Results<NoContent, ProblemHttpResult>> UpdateGithubToken(string token, ClaimsPrincipal userClaims);
    Task<Results<Ok<UserResponse>, ProblemHttpResult>> Get(ClaimsPrincipal userClaims);
}
