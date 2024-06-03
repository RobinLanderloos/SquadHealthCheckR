using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.Data.Configurations;

internal class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder
            .HasMany(s => s.SquadMembers)
            .WithMany(a => a.Sessions)
            .UsingEntity<SquadMembersSessions>(
                r => r.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.SquadMemberId),
                l => l.HasOne<Session>().WithMany().HasForeignKey(e => e.SessionId))
            .HasKey(s => new { s.SessionId, s.SquadMemberId});
    }

}