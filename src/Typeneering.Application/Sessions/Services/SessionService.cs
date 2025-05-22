using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Typeneering.Application.Base.Contracts.Requests;
using Typeneering.Application.Base.Contracts.Responses;
using Typeneering.Application.Sessions.Contracts.Requests;
using Typeneering.Application.Sessions.Contracts.Responses;
using Typeneering.Domain.Shared.Exceptions;
using Typeneering.Domain.Session.Entities;
using Typeneering.Domain.User.Entities;
using Typeneering.Infraestructure;
using Typeneering.Application.Base.Constants;
using Typeneering.Application.Sessions.Constants;
using Typeneering.Application.Handlers.Claims;
using Typeneering.Application.Sessions.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;

namespace Typeneering.Application.Sessions.Services;

public sealed class SessionService : ISessionService
{

    private readonly IUserContextHandler _userClaimsHandler;
    private readonly TypeneeringDbContext _dbContext;
    private readonly PostSessionValidator _postValidator;
    private readonly GetSessionValidator _getValidator;
    private readonly LeaderboardValidator _leaderboardValidator;

    public SessionService(
            TypeneeringDbContext dbContext,
            IUserContextHandler UserContextHandler
,
            PostSessionValidator postValidator,
            GetSessionValidator getValidator,
            LeaderboardValidator leaderboardValidator)
    {
        _dbContext = dbContext;
        _userClaimsHandler = UserContextHandler;
        _leaderboardValidator = leaderboardValidator;
        _postValidator = postValidator;
        _getValidator = getValidator;
        _leaderboardValidator = leaderboardValidator;
    }

    public async Task<Results<Ok<SessionResponse>, ProblemHttpResult>> Get(int id)
    {
        var session = await _dbContext.Sessions.Where(sessions => sessions.Id == id
                                                                    && sessions.UserId == _userClaimsHandler.UserId)
                                                .FirstOrDefaultAsync();

        if (session is null)
            return TypedResults.Problem(
                    title: BaseConsts.Errors.IdNotFoundTitle,
                    detail: BaseConsts.Errors.IdNotFoundDetail,
                    type: BaseConsts.Errors.IdNotFoundType,
                    statusCode: StatusCodes.Status404NotFound
                    );

        return TypedResults.Ok(new SessionResponse(session));
    }

    public async Task<Results<Ok<ResultResponse<SessionResponse>>, ProblemHttpResult>> GetList(GetSessionRequest requestQuery)
    {
        var validation = _getValidator.Validate(requestQuery);
        if (!validation.IsValid)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Type = nameof(GetSessionRequest),
                Title = BaseConsts.Errors.InvalidRequestTitle,
                Detail = BaseConsts.Errors.InvalidRequestDetail,
                Status = StatusCodes.Status400BadRequest,
                Errors = validation.ToDictionary()
            });

        var query = _dbContext.Sessions.Where(sess => sess.UserId == _userClaimsHandler.UserId
                                                        && sess.TotalCharacters > requestQuery.MinCharacters
                                                        && sess.CorrectCharacters > requestQuery.MinCorrectCharacters)
                                        .AsNoTracking();

        if (requestQuery.Filename is not null)
            query = query.Where(sess => sess.Filename == requestQuery.Filename);

        if (requestQuery.Filetypes is not null)
            query = query.Where(sess => requestQuery.Filetypes.Contains(sess.Filetype));

        if (requestQuery.BeginDate is not null)
            query = query.Where(sess => sess.CreatedAt > requestQuery.BeginDate);

        if (requestQuery.EndDate is not null)
            query = query.Where(sess => sess.CreatedAt < requestQuery.EndDate);

        if (requestQuery.MaxCorrectCharacters is not null)
            query = query.Where(sess => sess.CorrectCharacters < requestQuery.MaxCorrectCharacters);

        if (requestQuery.MaxCharacters is not null)
            query = query.Where(sess => sess.TotalCharacters < requestQuery.MaxCharacters);

        if (requestQuery.Seconds is not null)
            query = query.Where(sess => sess.Seconds == requestQuery.Seconds);

        query = query.Skip(requestQuery.Skip)
                        .Take(requestQuery.Take);

        var results = await query.Select(sess => new SessionResponse(sess)).ToListAsync();

        return TypedResults.Ok
            (
                new ResultResponse<SessionResponse>
                (
                    Title: BaseConsts.Results.RetrievedTitle,
                    Message: BaseConsts.Results.ItemsRetrieved(results.Count),
                    Data: results,
                    TotalCount: results.Count
                )
            );
    }

    public async Task<Results<CreatedAtRoute<SessionResponse>, ProblemHttpResult>> Insert(PostSessionRequest request)
    {
        var validation = _postValidator.Validate(request);
        if (!validation.IsValid)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Type = nameof(LeaderboardRequest),
                Title = BaseConsts.Errors.InvalidRequestTitle,
                Detail = BaseConsts.Errors.InvalidRequestDetail,
                Status = StatusCodes.Status400BadRequest,
                Errors = validation.ToDictionary()
            });

        var newSession = await _dbContext.Sessions.AddAsync(new SessionEntity
        {
            UserId = _userClaimsHandler.UserId,
            Seconds = request.Seconds,
            Filename = request.Filename,
            Filetype = request.Filename,
            CharactersTyped = request.CharactersTyped,
            TotalCharacters = request.TotalCharacters,
            CorrectCharacters = request.CorrectCharacters
        });

        await _dbContext.SaveChangesAsync();

        var sessionResponse = new SessionResponse(newSession.Entity);
        return TypedResults.CreatedAtRoute(sessionResponse, nameof(Get), new { newSession.Entity.Id });

    }

    public async Task<Results<Ok<ResultResponse<UserSessionResponse>>, ProblemHttpResult>> GetLeaderboard(LeaderboardRequest request)
    {
        var validation = _leaderboardValidator.Validate(request);
        if (!validation.IsValid)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Type = nameof(LeaderboardRequest),
                Title = BaseConsts.Errors.InvalidRequestTitle,
                Detail = BaseConsts.Errors.InvalidRequestDetail,
                Status = StatusCodes.Status400BadRequest,
                Errors = validation.ToDictionary()
            });

        var sessions = await _dbContext.Sessions.Include(sess => sess.User)
                                                .Skip(request.Skip)
                                                .Take(request.Take)
                                                .Select(sess => new UserSessionResponse(sess))
                                                .OrderByDescending(sess => sess.CorrectCharacters)
                                                .ThenByDescending(sess => sess.TotalCharacters)
                                                .ToListAsync();

        return TypedResults.Ok(
                new ResultResponse<UserSessionResponse>(
                    Title: SessionConsts.Leaderboard.Results.RetrievedLeaderboard,
                    Message: BaseConsts.Results.ItemsRetrieved(sessions.Count),
                    Data: sessions,
                    TotalCount: sessions.Count
                    ));
    }
}
