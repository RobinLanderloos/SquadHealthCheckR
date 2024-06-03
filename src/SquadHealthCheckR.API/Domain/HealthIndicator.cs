using Ardalis.GuardClauses;

namespace SquadHealthCheckR.API.Domain;

internal class HealthIndicator
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string Name { get; private set; }
    public string PositiveDescription { get; private set; }
    public string NegativeDescription { get; private set; }

    private readonly List<Vote> _votes = new();
    public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();

    public HealthIndicator(Guid? id, Guid sessionId,string name, string positiveDescription, string negativeDescription)
    {
        Id = id ?? Guid.Empty;
        SessionId = sessionId;
        Name = Guard.Against.NullOrWhiteSpace(name);
        PositiveDescription = Guard.Against.NullOrWhiteSpace(positiveDescription);
        NegativeDescription = Guard.Against.NullOrWhiteSpace(negativeDescription);
    }

    public void Vote(ApplicationUser squadMember, VoteSentiment sentiment)
    {
        var vote = new Vote(this, squadMember, sentiment);
        _votes.Add(vote);
    }

    // Required for EF
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private HealthIndicator(){}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
