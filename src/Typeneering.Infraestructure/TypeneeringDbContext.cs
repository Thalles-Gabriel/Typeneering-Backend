using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Typeneering.Domain.Preference.Entities;
using Typeneering.Domain.Session.Entities;
using Typeneering.Domain.User.Entities;
using Typeneering.Infraestructure.Extensions;

namespace Typeneering.Infraestructure;
public class TypeneeringDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
{
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<PreferenceEntity> Preferences { get; set; }
    public DbSet<UserPreferenceEntity> UserPreferences { get; set; }

    public TypeneeringDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        
        optionsBuilder.ConfigureDbContext();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureDbModel();
    }
}
