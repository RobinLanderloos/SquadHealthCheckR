using Microsoft.AspNetCore.Identity;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.UseCases.Admin;

internal static class DeleteUser
{
    public static RouteGroupBuilder MapDeleteUserEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapDelete("/users/{userId}", (UserManager<ApplicationUser> userManager, string userId) =>
        {
            var user = userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                return Results.NotFound();
            }

            var result = userManager.DeleteAsync(user).Result;

            return !result.Succeeded ? Results.BadRequest(result.Errors) : Results.Ok();
        });

        return builder;
    }
}