using Microsoft.AspNetCore.Identity;

namespace SquadHealthCheckR.API.Domain;

#pragma warning disable CS8618
internal class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole(string roleName) : base(roleName)
    {

    }

    // ReSharper disable once UnusedMember.Local
    private ApplicationRole(){}
}