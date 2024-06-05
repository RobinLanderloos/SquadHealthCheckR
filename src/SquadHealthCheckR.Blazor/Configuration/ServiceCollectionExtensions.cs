namespace SquadHealthCheckR.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<BackendOptions>(configuration.GetSection(BackendOptions.SectionName));

       return services;
    }
}