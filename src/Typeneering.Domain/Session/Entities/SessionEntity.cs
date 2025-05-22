using Typeneering.Domain.Base.Entities;

namespace Typeneering.Domain.Session.Entities;

public sealed class SessionEntity : UserRelationEntityBase
{
    public int CorrectCharacters { get; set; }
    public int CharactersTyped { get; set; }
    public int TotalCharacters { get; set; }
    public string Filename { get; set; }
    public string Filetype { get; set; }
    public int? Seconds { get; set; }
}
