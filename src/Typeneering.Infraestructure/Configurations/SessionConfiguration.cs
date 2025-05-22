using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Typeneering.Domain.Session.Constraints;
using Typeneering.Domain.Session.Entities;

namespace Typeneering.Infraestructure.Configurations;
public class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.ToTable("Sessions");

        builder.Property(session => session.Filename)
                .HasMaxLength(SessionConstraints.FilenameMaxLength);

        builder.Property(session => session.Filetype)
                .HasMaxLength(SessionConstraints.FiletypeMaxLength);

        builder.Property(session => session.CreatedAt)
                .HasDefaultValueSql("NOW()");

        builder.HasOne(session => session.User)
                .WithMany(user => user.Sessions)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}
