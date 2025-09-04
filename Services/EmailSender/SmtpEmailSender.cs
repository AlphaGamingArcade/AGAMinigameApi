using System.Net;
using System.Net.Mail;
using AGAMinigameApi.Services.EmailSender;
using Microsoft.Extensions.Options;
using SmptOptions;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;

    public SmtpEmailSender(IOptions<SmtpOptions> opt)
        => _opt = opt.Value;

    public async Task SendVerificationEmailAsync(string toEmail, string displayName, string verificationLink)
    {
        using var msg = new MailMessage
        {
            From = new MailAddress(_opt.FromEmail, _opt.FromName),
            Subject = "Verify your email",
            Body = BuildHtmlBody(displayName, verificationLink),
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress(toEmail, displayName));

        // optional plain-text alt
        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            BuildTextBody(displayName, verificationLink), null, "text/plain"));

        using var client = new SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.EnableSsl,
            Credentials = new NetworkCredential(_opt.User, _opt.Password)
        };

        await client.SendMailAsync(msg);
    }

    private static string BuildHtmlBody(string name, string link) => $@"
        <!DOCTYPE html>
        <html>
        <body style=""font-family:Segoe UI, Roboto, Arial, sans-serif; color:#1f2937; line-height:1.5;"">
            <h2 style=""margin:0 0 12px"">Confirm your email</h2>
            <p>Hi {WebUtility.HtmlEncode(name)},</p>
            <p>Thanks for registering. Please confirm your email by clicking the button below:</p>
            <p style=""margin:20px 0"">
            <a href=""{WebUtility.HtmlEncode(link)}""
                style=""background:#2563eb;color:#fff;padding:10px 16px;border-radius:6px;text-decoration:none;display:inline-block;"">
                Verify Email
            </a>
            </p>
            <p>If the button doesn't work, copy this URL into your browser:</p>
            <p><a href=""{WebUtility.HtmlEncode(link)}"">{WebUtility.HtmlEncode(link)}</a></p>
            <p style=""color:#6b7280;font-size:12px"">This link may expire soon.</p>
        </body>
        </html>";

    private static string BuildTextBody(string name, string link)
        => $"Hi {name},\n\nPlease verify your email by opening this link:\n{link}\n\nIf you didn't request this, you can ignore this message.";
}
