using Microsoft.AspNetCore.Authorization;

namespace SquadHealthCheckR.API.Auth;

public static class AuthorizationPolicies
{
    public static AuthorizationPolicy AdminOnly { get; } =
        new AuthorizationPolicyBuilder().RequireRole(Roles.Admin).Build();
}