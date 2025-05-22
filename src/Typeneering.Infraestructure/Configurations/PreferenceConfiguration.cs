using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Typeneering.Domain.Preference.Constraints;
using Typeneering.Domain.Preference.Entities;

namespace Typeneering.Infraestructure.Configurations;

public class PreferenceConfiguration : IEntityTypeConfiguration<PreferenceEntity>
{
    public void Configure(EntityTypeBuilder<PreferenceEntity> builder)
    {
        builder.ToTable("Preferences");

        builder.HasIndex(pref => pref.Name)
                .IsUnique();

        builder.Property(pref => pref.CreatedAt)
                .HasDefaultValueSql("NOW()");

        builder.Property(pref => pref.Name)
                .HasMaxLength(PreferenceConstraints.NameMaxLength);
    }
}
