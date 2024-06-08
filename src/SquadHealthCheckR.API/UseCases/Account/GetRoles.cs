using System.Security.Claims;

namespace SquadHealthCheckR.API.UseCases.Account;

internal static class GetRoles
{
    public static RouteGroupBuilder MapGetRolesEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapGet("/roles", (ClaimsPrincipal user) =>
        {
            if (user.Identity is null || !user.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }

            var identity = (ClaimsIdentity)user.Identity;
            var roles = identity.FindAll(identity.RoleClaimType)
                .Select(c =>
                    new
                    {
                        c.Issuer,
                        c.OriginalIssuer,
                        c.Type,
                        c.Value,
                        c.ValueType
                    });

            return TypedResults.Json(roles);
        }).RequireAuthorization();

        return builder;
    }
}