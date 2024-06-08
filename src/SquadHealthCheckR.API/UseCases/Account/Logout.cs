using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.UseCases.Account;

internal static class Logout
{
    public static RouteGroupBuilder MapLogoutEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager, [FromBody] object? empty) =>
        {
            if (empty is null) return Results.Unauthorized();

            await signInManager.SignOutAsync();

            return Results.Ok();
        }).RequireAuthorization();

        return builder;
    }
}