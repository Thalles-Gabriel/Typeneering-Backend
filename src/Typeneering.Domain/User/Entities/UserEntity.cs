using Microsoft.AspNetCore.Identity;
using Typeneering.Domain.Base.Entities;
using Typeneering.Domain.Preference.Entities;
using Typeneering.Domain.Session.Entities;

namespace Typeneering.Domain.User.Entities;

public sealed class UserEntity : IdentityUser<Guid>
{
    public string? GitHubToken { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public new string UserName { get; set; }
    public new string NormalizedUserName { get; set; }
    public ICollection<SessionEntity> Sessions { get; set; } = [];
    public ICollection<PreferenceEntity> Preferences { get; set; } = [];
}
