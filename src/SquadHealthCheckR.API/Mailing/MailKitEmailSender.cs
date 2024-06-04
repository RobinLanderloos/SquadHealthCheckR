using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using SquadHealthCheckR.API.Domain;

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

internal class MailKitEmailSender : IEmailSender<ApplicationUser>
{
    private readonly EmailOptions _emailOptions;

    public MailKitEmailSender(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var message = new MimeMessage()
        {
            Subject = "Confirm your email",
            Body = new BodyBuilder
            {
                HtmlBody= $"Please confirm your email by clicking this link: <a href=\"{confirmationLink}\">Confirm Email</a>"
            }.ToMessageBody(),
            To = { new MailboxAddress(user.UserName ?? email, email) },
            From = { new MailboxAddress(_emailOptions.DefaultFromName, _emailOptions.DefaultFromEmail) }
        };

        await SendEmail(message);
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var message = new MimeMessage()
        {
            Subject = "Reset your password",
            Body = new BodyBuilder
            {
                HtmlBody= $"Please reset your password by clicking this link: <a href=\"{resetLink}\">Reset Password</a>"
            }.ToMessageBody(),
            To = { new MailboxAddress(user.UserName ?? email, email) },
            From = { new MailboxAddress(_emailOptions.DefaultFromName, _emailOptions.DefaultFromEmail) }
        };

        return SendEmail(message);
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var message = new MimeMessage()
        {
            Subject = "Reset your password",
            Body = new BodyBuilder
            {
                HtmlBody= $"Your reset code is: <b>{resetCode}</b>"
            }.ToMessageBody(),
            To = { new MailboxAddress(user.UserName ?? email, email) },
            From = { new MailboxAddress(_emailOptions.DefaultFromName, _emailOptions.DefaultFromEmail) }
        };

        return SendEmail(message);
    }

    private async Task SendEmail(MimeMessage message)
    {
        using var client = new SmtpClient();

        await client.ConnectAsync(_emailOptions.Host, 465);
        await client.AuthenticateAsync(_emailOptions.DefaultFromEmail, _emailOptions.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}