using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.Data.Configurations;

internal class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasMany<ApplicationUser>();
        builder.HasMany<HealthIndicator>();
        builder.HasMany<Vote>();
    }

}