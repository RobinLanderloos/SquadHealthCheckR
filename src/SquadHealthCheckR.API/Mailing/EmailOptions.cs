using System.ComponentModel.DataAnnotations;

namespace SquadHealthCheckR.API.Mailing;

internal class EmailOptions
{
    public const string SectionName = "Mailing";

    [Required]
    public string Host { get; set; }
    [Required]
    public int Port{ get; set; }
    public string Password{ get; set; }
    [Required]
    public string DefaultFromEmail { get; set; }
    [Required]
    public string DefaultFromName { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EmailOptions(){}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}