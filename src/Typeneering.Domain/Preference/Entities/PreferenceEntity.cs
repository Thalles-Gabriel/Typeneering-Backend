using Typeneering.Domain.Base.Entities;
using Typeneering.Domain.User.Entities;
namespace Typeneering.Domain.Preference.Entities;

public sealed class PreferenceEntity : BaseEntity
{
    public string Name { get; set; }
    public ICollection<UserEntity> Users { get; set; } = [];
}
