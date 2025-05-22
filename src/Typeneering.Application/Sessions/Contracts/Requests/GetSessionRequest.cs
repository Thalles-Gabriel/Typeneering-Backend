using Typeneering.Application.Base.Contracts.Requests;

namespace Typeneering.Application.Sessions.Contracts.Requests;

public sealed record GetSessionRequest
(
    int? MaxCharacters,
    int? MaxCorrectCharacters,
    string[]? Filetypes,
    string? Filename,
    DateTimeOffset? BeginDate,
    DateTimeOffset? EndDate,
    int? Seconds,
    int MinCorrectCharacters = 0,
    int MinCharacters = 0
) : PagedResultRequest;
