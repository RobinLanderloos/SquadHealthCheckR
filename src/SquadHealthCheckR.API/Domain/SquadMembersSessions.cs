namespace SquadHealthCheckR.API.Domain;

internal class SquadMembersSessions
{
    public Guid SessionId { get; private set; }
    public Guid SquadMemberId { get; private set; }
    public SessionUserType Type { get; private set; }

    public enum SessionUserType
    {
        SquadLeader,
        SquadMember
    }

    private SquadMembersSessions(){}
}