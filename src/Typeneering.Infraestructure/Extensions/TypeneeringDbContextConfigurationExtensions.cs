using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Typeneering.Domain.Preference.Entities;
using Typeneering.Domain.Shared.Exceptions;
using Typeneering.Infraestructure.Configurations;
using Typeneering.Infraestructure.Seeding;

namespace Typeneering.Infraestructure.Extensions;

public static class TypeneeringDbContextConfigurationExtensions
{
    public static string ConnectionString = new ConfigurationManager().SetBasePath(AppContext.BaseDirectory)
                                                    .AddJsonFile("appsettings.json")
                                                    .AddJsonFile("appsettings.Development.json")
                                                    .AddJsonFile("appsettings.Production.json")
                                                    .Build()
                                                    .GetConnectionString("postgresConn") ?? throw new InvalidProjectConfigurationException();

    public static void ConfigureDbModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<PreferenceEntity>()
                    .HasMany(pref => pref.Users)
                    .WithMany(user => user.Preferences)
                    .UsingEntity<UserPreferenceEntity>();
    }

    public static void ConfigureDbContext(this DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(ConnectionString)
                            .UseSeeding((context, _) =>
                            {
                                var seeding = new DataSeeding();
                                foreach (var item in seeding.PreferencesData)
                                {
                                    var entryExists = context.Set<PreferenceEntity>().Any(pref => pref.Name == item.Name);

                                    if (!entryExists)
                                        context.Set<PreferenceEntity>().Add(item);
                                }

                                context.SaveChanges();
                            })
                            .UseAsyncSeeding(async (context, _, token) =>
                            {

                                var seeding = new DataSeeding();
                                foreach (var item in seeding.PreferencesData)
                                {
                                    var entryExists = await context.Set<PreferenceEntity>().AnyAsync(pref => pref.Name == item.Name, token);

                                    if (!entryExists)
                                        await context.Set<PreferenceEntity>().AddAsync(item, token);
                                }

                                await context.SaveChangesAsync(token);
                            });


}
