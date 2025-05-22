using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Typeneering.Domain.Preference.Constraints;
using Typeneering.Domain.Preference.Entities;

namespace Typeneering.Infraestructure.Configurations;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreferenceEntity>
{
    public void Configure(EntityTypeBuilder<UserPreferenceEntity> builder)
    {
        builder.ToTable("User_Preferences");

        builder.Property(upref => upref.Value)
                .HasMaxLength(UserPreferenceConstraints.ValueMaxLength);

        builder.Property(upref => upref.CreatedAt)
                .HasDefaultValueSql("NOW()");
    }
}
