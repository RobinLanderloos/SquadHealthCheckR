using Ardalis.GuardClauses;
using SquadHealthCheckR.API.Data;

namespace SquadHealthCheckR.API.Domain;

internal class Vote
{
    public Guid HealthIndicatorId { get; private set; }
    public HealthIndicator HealthIndicator { get; private set; }
    public ApplicationUser SquadMember { get; private set; }
    public Guid SquadMemberId { get; private set; }
    public VoteSentiment VoteSentiment { get; private set; }
    public TrendSentiment TrendSentiment { get; private set; }

    public Vote(HealthIndicator healthIndicator, ApplicationUser squadMember, VoteSentiment voteSentiment, TrendSentiment trendSentiment = TrendSentiment.Stale)
    {
        HealthIndicator = healthIndicator;
        SquadMember = squadMember;
        VoteSentiment = Guard.Against.EnumOutOfRange(voteSentiment);
        TrendSentiment = Guard.Against.EnumOutOfRange(trendSentiment);
    }

    // Required for EF
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Vote(){}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public enum TrendSentiment
{
    Upwards,
    Stale,
    Downwards
}

public enum VoteSentiment
{
    Positive,
    Negative
}