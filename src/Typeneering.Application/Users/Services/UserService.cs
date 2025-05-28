using System.Collections.Immutable;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Typeneering.Application.Base.Contracts.Responses;
using Typeneering.Application.Users.Constants;
using Typeneering.Application.Handlers.Token;
using Typeneering.Application.Users.Contracts.Requests;
using Typeneering.Domain.Shared.Exceptions;
using Typeneering.Domain.Shared;
using Typeneering.Domain.User.Entities;
using Typeneering.Infraestructure;
using Typeneering.Application.Users.Validators;
using Typeneering.Application.Base.Constants;
using Typeneering.Application.Users.Contracts.Responses;
using Typeneering.Application.UserPreferences.Contracts.Responses;

namespace Typeneering.Application.Users.Services;

public sealed class UserService : IUserService
{
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IJWTTokenHandler _tokenHandler;
    private readonly IUserStore<UserEntity> _userStore;
    private readonly TypeneeringDbContext _dbContext;
    private readonly GitHubTokenValidator _ghValidator;

    public UserService(
            SignInManager<UserEntity> signInManager,
            IOptions<JwtOptions> jwtOptions,
            UserManager<UserEntity> userManager,
            IJWTTokenHandler tokenHandler,
            IUserStore<UserEntity> userStore,
            TypeneeringDbContext dbContext,
            GitHubTokenValidator ghValidator)
    {
        _signInManager = signInManager;
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _tokenHandler = tokenHandler;
        _userStore = userStore;
        _dbContext = dbContext;
        _ghValidator = ghValidator;
    }

    public async Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Register(UserRegisterRequest login)
    {
        var user = new UserEntity
        {
            Email = login.Email,
            UserName = login.Username,
            NormalizedUserName = login.Nickname ?? login.Username
        };

        var result = await _userManager.CreateAsync(user, login.Password);

        if (!result.Succeeded)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Errors = result.Errors.ToDictionary(keyValue => keyValue.Code, keyValue => new string[] { keyValue.Description }),
                Title = UserConsts.Errors.Login.RegisterTitle,
                Detail = UserConsts.Errors.Login.RegisterDetails,
                Type = UserConsts.Errors.Login.RegisterType,
                Status = StatusCodes.Status400BadRequest
            });

        return TypedResults.Ok(new ResultResponse(
                        UserConsts.Results.User.CreatedTitle,
                        UserConsts.Results.User.CreatedMessage
                    ));
    }

    public async Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> Login(UserLoginRequest login)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent: false, lockoutOnFailure: true);

        if (!signInResult.Succeeded)
            return TypedResults.Problem(
                    detail: UserConsts.Errors.Login.LoginDetail,
                    title: UserConsts.Errors.Login.LoginTitle,
                    type: UserConsts.Errors.Login.LoginType,
                    statusCode: StatusCodes.Status422UnprocessableEntity
                    );

        var user = await _userManager.FindByNameAsync(login.UserName) ?? throw new UnreachableException();
        var tokenResponse = _tokenHandler.GenerateAccessToken(user);
        return TypedResults.Ok(tokenResponse);
    }

    public async Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> Refresh(RefreshRequest refreshRequest)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _jwtOptions.SigningCredentials.Key,
            ValidateLifetime = true,
            ValidAudience = _jwtOptions.Audience,
            ValidIssuer = _jwtOptions.Issuer
        };

        var principalClaims = new JwtSecurityTokenHandler()
                                .ValidateToken(refreshRequest.RefreshToken,
                                                tokenValidationParameters,
                                                out var securityToken);

        if (securityToken is not JwtSecurityToken || principalClaims is null)
            return TypedResults.Problem(title: UserConsts.Errors.Token.InvalidTokenTitle,
                                        detail: UserConsts.Errors.Token.InvalidTokenDetail,
                                        type: UserConsts.Errors.Token.InvalidTokenType,
                                        statusCode: StatusCodes.Status400BadRequest);

        var user = await _userManager.FindByNameAsync(principalClaims.Identity?.Name ?? throw new InvalidUserClaimsException());

        if (user is null)
            return TypedResults.Problem(title: UserConsts.Errors.Token.InvalidTokenInformationTitle,
                                        detail: UserConsts.Errors.Token.InvalidaTokenInformationDetail,
                                        type: UserConsts.Errors.Token.InvalidTokenInformationType,
                                        statusCode: StatusCodes.Status403Forbidden);

        var accessTokenResponse = _tokenHandler.GenerateAccessToken(user);

        return TypedResults.Ok(accessTokenResponse);
    }

    public async Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Update(PatchUserRequest userRequest, ClaimsPrincipal userClaims)
    {
        if (userRequest.GithubToken is not null)
        {
            var validation = _ghValidator.Validate(userRequest.GithubToken);
            if (!validation.IsValid)
                return TypedResults.Problem(new ValidationProblemDetails
                {
                    Type = nameof(UserEntity.GitHubToken),
                    Title = BaseConsts.Errors.InvalidRequestTitle,
                    Detail = BaseConsts.Errors.InvalidRequestDetail,
                    Status = StatusCodes.Status400BadRequest,
                    Errors = validation.ToDictionary()
                });
        }

        var user = await _userManager.GetUserAsync(userClaims) ?? throw new InvalidUserClaimsException();

        if (user is null)
            return TypedResults.Problem(
                    title: UserConsts.Errors.User.UpdateTitle,
                    detail: UserConsts.Errors.User.NullDetail,
                    type: UserConsts.Errors.User.NullType,
                    statusCode: StatusCodes.Status404NotFound
                    );

        user.UserName = userRequest.Username ?? user.UserName;
        user.Email = userRequest.Email ?? user.Email;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var identityResult = await _userStore.UpdateAsync(user, CancellationToken.None);

        if (!identityResult.Succeeded)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Errors = identityResult.Errors.ToDictionary(keyValue => keyValue.Code, keyValue => new string[] { keyValue.Description }),
                Title = UserConsts.Errors.User.UpdateTitle,
                Detail = UserConsts.Errors.User.UpdateDetail,
                Type = UserConsts.Errors.User.UpdateType,
                Status = StatusCodes.Status400BadRequest
            });

        return TypedResults.Ok(new ResultResponse(
                    UserConsts.Results.User.UpdateTitle,
                    UserConsts.Results.User.UpdateDetail
                    ));
    }

    public async Task<Results<Ok<ResultResponse>, ProblemHttpResult>> Delete(ClaimsPrincipal userClaims)
    {
        var user = await _userManager.GetUserAsync(userClaims) ?? throw new InvalidUserClaimsException();

        var identityResult = await _userStore.DeleteAsync(user, CancellationToken.None);

        if (!identityResult.Succeeded)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Errors = identityResult.Errors.ToDictionary(keyValue => keyValue.Code, keyValue => new string[] { keyValue.Description }),
                Title = UserConsts.Errors.User.DeleteTitle,
                Detail = UserConsts.Errors.User.DeleteDetail,
                Type = UserConsts.Errors.User.DeleteType,
                Status = StatusCodes.Status422UnprocessableEntity
            });

        return TypedResults.Ok(new ResultResponse(
                    UserConsts.Results.User.DeleteTitle,
                    UserConsts.Results.User.DeleteDetail
                    ));
    }

    public async Task<Results<NoContent, ProblemHttpResult>> UpdateGithubToken(string token, ClaimsPrincipal userClaims)
    {
        var validation = _ghValidator.Validate(token);
        if (!validation.IsValid)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Type = nameof(UserEntity.GitHubToken),
                Title = BaseConsts.Errors.InvalidRequestTitle,
                Detail = BaseConsts.Errors.InvalidRequestDetail,
                Status = StatusCodes.Status400BadRequest,
                Errors = validation.ToDictionary()
            });

        var dbUser = await _userManager.GetUserAsync(userClaims) ?? throw new InvalidUserClaimsException();

        var rowsAffected = await _dbContext.Users.Where(user => user.Id == dbUser.Id)
                                    .ExecuteUpdateAsync(entity =>
                                            entity.SetProperty(user => user.GitHubToken, token));

        if (rowsAffected < 1)
            return TypedResults.Problem(
                    title: UserConsts.Errors.User.DeleteDetail,
                    detail: UserConsts.Errors.User.NullDetail,
                    type: UserConsts.Errors.User.NullType,
                    statusCode: StatusCodes.Status422UnprocessableEntity
                    );

        return TypedResults.NoContent();
    }

    public async Task<Results<Ok<UserResponse>, ProblemHttpResult>> Get(ClaimsPrincipal userClaims)
    {
        var dbUser = await _userManager.GetUserAsync(userClaims) ?? throw new InvalidUserClaimsException();
        var userPreferences = await _dbContext.UserPreferences.Where(upref => upref.UserId == dbUser.Id)
                                                                .Include(upref => upref.Preference)
                                                                .Select(upref => new UserPreferenceResponse(upref.Preference.Name, upref.Value))
                                                                .ToListAsync();

        return TypedResults.Ok(new UserResponse(dbUser.NormalizedUserName, dbUser.GitHubToken, userPreferences));
    }
}
