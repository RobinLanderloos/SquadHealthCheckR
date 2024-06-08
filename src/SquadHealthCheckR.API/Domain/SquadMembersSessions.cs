namespace SquadHealthCheckR.API.Domain;

internal class SquadMembersSessions
{
    public Guid SessionId { get; private set; }
    public Session? Session { get; private set; }
    public Guid SquadMemberId { get; private set; }
    public ApplicationUser? SquadMember { get; private set; }
    public SessionUserType Type { get; private set; }

    internal SquadMembersSessions(Session session, ApplicationUser squadMember, SessionUserType type = SessionUserType.SquadMember)
    {
        Session = session;
        SquadMember = squadMember;
        Type = type;
    }


    public enum SessionUserType
    {
        SquadMember,
        SquadLeader
    }

    private SquadMembersSessions(){}
}