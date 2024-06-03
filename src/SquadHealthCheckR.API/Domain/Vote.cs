using Ardalis.GuardClauses;
using SquadHealthCheckR.API.Data;

namespace SquadHealthCheckR.API.Domain;

internal class Vote
{
    public Guid Id { get; private set; }
    public Guid HealthIndicatorId { get; private set; }
    public HealthIndicator HealthIndicator { get; private set; }
    public ApplicationUser SquadMember { get; private set; }
    public Guid SquadMemberId { get; private set; }
    public VoteSentiment Sentiment { get; private set; }

    public Vote(Guid? id, HealthIndicator healthIndicator, ApplicationUser squadMember, VoteSentiment sentiment = VoteSentiment.Stale)
    {
        Id = id ?? Guid.Empty;
        HealthIndicator = healthIndicator;
        SquadMember = squadMember;
        Sentiment = Guard.Against.EnumOutOfRange(sentiment);
    }

    // Required for EF
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Vote(){}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public enum VoteSentiment
{
    Upwards,
    Stale,
    Downwards
}