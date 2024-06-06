using Microsoft.AspNetCore.Identity;
using SquadHealthCheckR.API.Auth;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.Bootstrapper;

public class AdminBootstrapper
{
    public static async Task InitializeAdminUserAndRoleIfNotExists(WebApplication webApplication)
    {
        var innerScope = webApplication.Services.CreateScope();

        var userManager = innerScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = innerScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = innerScope.ServiceProvider.GetRequiredService<ILogger<AdminBootstrapper>>();

        await InitializeAdminRole(roleManager, logger);
        await InitializeAdminUser(userManager, webApplication.Configuration, logger);
    }

    private static async Task InitializeAdminUser(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AdminBootstrapper> logger)
    {

        var username = "admin";

        var user = await userManager.FindByNameAsync(username);

        if (user is null)
        {
            logger.LogDebug("The admin user wasn't created yet, creating now...");

            var adminPassword = configuration["Admin:Password"];
            var adminEmail = configuration["Admin:Email"];

            if (adminEmail == null || adminPassword == null)
            {
                throw new NullReferenceException("The required admin credentials were not provided");
            }

            user = new ApplicationUser()
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createUserResult = await userManager.CreateAsync(user, adminPassword);

            if (createUserResult != IdentityResult.Success)
            {
                var errors = createUserResult.Errors.Aggregate(string.Empty, (builder, error) =>
                {
                    builder += $"{error.Code} - {error.Description}\r\n";
                    return builder;
                });
                logger.LogError("An error occurred while creating the admin user: {Errors}", errors);
            }

            logger.LogDebug("The admin user was created");
        }

        var hasAdminRole = await userManager.IsInRoleAsync(user, Roles.Admin);
        if (!hasAdminRole)
        {
            logger.LogDebug("Adding admin user to admin role");
            await userManager.AddToRoleAsync(user, Roles.Admin);
            logger.LogDebug("The admin user was added to the admin role");
        }
    }

    private static async Task InitializeAdminRole(RoleManager<ApplicationRole> roleManager, ILogger<AdminBootstrapper> logger)
    {

        var role = await roleManager.FindByNameAsync(Roles.Admin);

        if (role == null)
        {
            logger.LogDebug("The admin role wasn't created yet, creating now...");

            role = new ApplicationRole(Roles.Admin);
            await roleManager.CreateAsync(role);

            logger.LogDebug("The admin role was created");
        }
    }
}