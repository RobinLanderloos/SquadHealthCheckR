using Ardalis.GuardClauses;

namespace SquadHealthCheckR.API.Domain;

public class HealthIndicator
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string PositiveDescription { get; private set; }
    public string NegativeDescription { get; private set; }

    public HealthIndicator(Guid? id, string name, string positiveDescription, string negativeDescription)
    {
        Id = id ?? Guid.Empty;
        Name = Guard.Against.NullOrWhiteSpace(name);
        PositiveDescription = Guard.Against.NullOrWhiteSpace(positiveDescription);
        NegativeDescription = Guard.Against.NullOrWhiteSpace(negativeDescription);
    }

    // Required for EF
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private HealthIndicator(){}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
