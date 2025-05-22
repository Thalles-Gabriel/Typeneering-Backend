using Typeneering.Domain.Session.Entities;

namespace Typeneering.Application.Sessions.Contracts.Responses;

public abstract record AbstractSessionResponse(SessionEntity Session)
{
    public int TotalCharacters { get; init; } = Session.TotalCharacters;
    public int CorrectCharacters { get; init; } = Session.CorrectCharacters;
    public int CharactersTyped { get; init; } = Session.CharactersTyped;
    public string Filename { get; init; } = Session.Filename;
    public string FileType { get; init; } = Session.Filetype;
    public DateTimeOffset SessionDate { get; init; } = Session.CreatedAt;
    public int? Seconds { get; set; } = Session.Seconds;
};
