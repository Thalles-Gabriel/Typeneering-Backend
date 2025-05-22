using Microsoft.AspNetCore.Identity;
using Typeneering.Domain.User.Entities;
using Typeneering.Infraestructure;

namespace Typeneering.HostApi.Extensions;

public static class IdentityExtensions
{
    public static IdentityBuilder AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddIdentity<UserEntity, IdentityRole<Guid>>()
                    .AddSignInManager<SignInManager<UserEntity>>()
                    .AddUserManager<UserManager<UserEntity>>()
                    .AddEntityFrameworkStores<TypeneeringDbContext>();

}
