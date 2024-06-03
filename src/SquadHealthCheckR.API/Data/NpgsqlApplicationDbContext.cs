using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SquadHealthCheckR.API.Domain;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace SquadHealthCheckR.API.Data;

internal class NpgsqlApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Session> Sessions { get; set; }

    public DbSet<HealthIndicator> HealthIndicators { get; set; }

    public DbSet<Vote> Votes { get; set; }

    public DbSet<SquadMembersSessions> SessionApplicationUsers { get; set; }

    public NpgsqlApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(NpgsqlApplicationDbContext).Assembly);
    }
}