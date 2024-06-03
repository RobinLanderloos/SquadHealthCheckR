using Microsoft.AspNetCore.Identity;

namespace SquadHealthCheckR.API.Domain;

#pragma warning disable CS8618
internal class ApplicationUser : IdentityUser<Guid>
{
    private List<Session> _sessions = new();
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();
}