namespace SquadHealthCheckR.Configuration;

public class BackendOptions
{
    public const string SectionName = "Backend";
    public Api Api { get; set; } = null!;
}

public class Api
{
    public const string SectionName = "Backend:Api";
    public string BaseUrl { get; set; } = null!;

}