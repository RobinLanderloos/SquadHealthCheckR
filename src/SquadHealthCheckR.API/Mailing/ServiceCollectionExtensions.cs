using Microsoft.AspNetCore.Identity;
using SquadHealthCheckR.API.Domain;

namespace SquadHealthCheckR.API.Mailing;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IEmailSender<ApplicationUser>, MailKitEmailSender>()
            .Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

       return services;
    }
}