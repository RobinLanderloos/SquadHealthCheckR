using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.Data.Configurations;

internal class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(v => new { v.SquadMemberId, v.HealthIndicatorId });
    }
}