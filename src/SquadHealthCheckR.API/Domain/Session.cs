using Ardalis.GuardClauses;
using SquadHealthCheckR.API.Data;

namespace SquadHealthCheckR.API.Domain;

internal class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    
    public Guid SquadLeaderId { get; private set; }
    public ApplicationUser SquadLeader { get; private set; }
    private readonly List<ApplicationUser> _squadMembers = [];
    public IReadOnlyCollection<ApplicationUser> SquadMembers => _squadMembers.AsReadOnly();

    // We leave this one open as we won't be operating on any operations on this collection
    public List<HealthIndicator> HealthIndicators { get; private set; } = [];

    private readonly List<Vote> _votes = new();
    public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();

    public string InviteCode { get; private set; } = "12345";

    public Session(Guid? id, ApplicationUser squadLeader, string name)
    {
        Id = id ?? Guid.Empty;
        SquadLeader = squadLeader;
        Name = Guard.Against.NullOrWhiteSpace(name);
    }

    public void JoinSquadMember(ApplicationUser member)
    {
        // TODO: Squad member joined DE
        _squadMembers.Add(member);
    }

    public void RemoveSquadMember(ApplicationUser member)
    {
        // TODO: Squad member left DE
        _squadMembers.Remove(member);
    }

    public void Vote(HealthIndicator healthIndicator, ApplicationUser squadMember, VoteSentiment sentiment)
    {
        var vote = new Vote(null, healthIndicator, squadMember, sentiment);
        _votes.Add(vote);
        // TODO: Squad member voted DE
    }

    // Required for Entity Framework
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Session(){}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}