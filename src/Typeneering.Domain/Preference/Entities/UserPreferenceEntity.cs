using Typeneering.Domain.Base.Entities;
using Typeneering.Domain.User.Entities;

namespace Typeneering.Domain.Preference.Entities;

public class UserPreferenceEntity : BaseEntity
{
    public string Value { get; set; }
    public UserEntity User { get; set; }
    public Guid UserId { get; set; }
    public PreferenceEntity Preference { get; set; }
    public int PreferenceId { get; set; }
}
