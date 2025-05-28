using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Typeneering.Domain.User.Constraints;
using Typeneering.Domain.User.Entities;

namespace Typeneering.Infraestructure.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.GitHubToken)
                .HasMaxLength(UserConstraints.GithubTokenMaxLength);

        builder.Property(user => user.UserName)
                .HasMaxLength(UserConstraints.UsernameMaxLength)
                .IsRequired();

        builder.Property(user => user.NormalizedUserName)
                .HasMaxLength(UserConstraints.UsernameMaxLength);

        builder.Property(user => user.CreatedAt)
                .HasDefaultValueSql("NOW()");
    }
}
