using Typeneering.Domain.Base.Entities;
using Typeneering.Domain.User.Entities;
namespace Typeneering.Domain.Base.Entities;

public class UserRelationEntityBase : BaseEntity
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
}
