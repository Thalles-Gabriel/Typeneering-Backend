namespace Typeneering.Application.Sessions.Contracts.Requests;

public sealed record PostSessionRequest
(
    int TotalCharacters,
    int CorrectCharacters,
    int CharactersTyped,
    string Filename,
    string Filetype,
    int? Seconds
);
