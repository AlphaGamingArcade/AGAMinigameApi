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
            Body = BuildVerificationHtmlBody(displayName, verificationLink),
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress(toEmail, displayName));

        // optional plain-text alt
        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            BuildVerificationTextBody(displayName, verificationLink), null, "text/plain"));

        using var client = new SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.EnableSsl,
            Credentials = new NetworkCredential(_opt.User, _opt.Password)
        };

        await client.SendMailAsync(msg);
    }

    public async Task SendForgotPasswordAsync(string toEmail, string displayName, string resetLink)
    {
        using var msg = new MailMessage
        {
            From = new MailAddress(_opt.FromEmail, _opt.FromName),
            Subject = "Reset Your Password",
            Body = BuildForgotPasswordHtmlBody(displayName, resetLink),
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress(toEmail, displayName));

        // optional plain-text alt
        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            BuildForgotPasswordTextBody(displayName, resetLink), null, "text/plain"));

        using var client = new SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.EnableSsl,
            Credentials = new NetworkCredential(_opt.User, _opt.Password)
        };

        await client.SendMailAsync(msg);
    }

    private static string BuildVerificationHtmlBody(string name, string link) => $@"
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

    private static string BuildForgotPasswordHtmlBody(string name, string resetLink) => $@"
        <!DOCTYPE html>
        <html>
        <body style=""font-family:Segoe UI, Roboto, Arial, sans-serif; color:#1f2937; line-height:1.5; max-width:600px; margin:0 auto; padding:20px;"">
            <div style=""text-align:center; margin-bottom:30px;"">
                <h1 style=""color:#dc2626; margin:0 0 8px; font-size:28px;"">🔐 Password Reset</h1>
                <p style=""color:#6b7280; margin:0; font-size:14px;"">Secure your account with a new password</p>
            </div>
            
            <div style=""background:#f9fafb; border-radius:8px; padding:24px; margin:20px 0;"">
                <h2 style=""margin:0 0 16px; color:#1f2937; font-size:20px;"">Hi {WebUtility.HtmlEncode(name)},</h2>
                <p style=""margin:0 0 16px;"">We received a request to reset your password. Click the button below to create a new password:</p>
                
                <div style=""text-align:center; margin:24px 0;"">
                    <a href=""{WebUtility.HtmlEncode(resetLink)}""
                        style=""background:#dc2626; color:#ffffff; padding:12px 24px; border-radius:6px; text-decoration:none; display:inline-block; font-weight:600; font-size:16px; box-shadow:0 2px 4px rgba(220, 38, 38, 0.3);"">
                        Reset Password
                    </a>
                </div>
                
                <div style=""background:#ffffff; border:1px solid #e5e7eb; border-radius:6px; padding:16px; margin:20px 0;"">
                    <p style=""margin:0 0 8px; font-size:14px; color:#6b7280;""><strong>Security Notice:</strong></p>
                    <ul style=""margin:0; padding-left:20px; color:#6b7280; font-size:14px;"">
                        <li>This link expires in 5 minutes for security</li>
                        <li>If you didn't request this reset, please ignore this email</li>
                        <li>Never share this link with anyone</li>
                    </ul>
                </div>
            </div>
            
            <div style=""border-top:1px solid #e5e7eb; padding-top:20px; margin-top:30px;"">
                <p style=""margin:0 0 8px; font-size:14px; color:#6b7280;""><strong>Link not working?</strong> Copy and paste this URL into your browser:</p>
                <p style=""background:#f3f4f6; padding:8px; border-radius:4px; word-break:break-all; font-size:12px; color:#4b5563; margin:8px 0 16px;"">
                    {WebUtility.HtmlEncode(resetLink)}
                </p>
                
                <p style=""color:#9ca3af; font-size:12px; text-align:center; margin:20px 0 0;"">
                    This is an automated message. Please do not reply to this email.<br>
                    If you need help, contact our support team.
                </p>
            </div>
        </body>
        </html>";

    private static string BuildVerificationTextBody(string name, string link)
        => $"Hi {name},\n\nPlease verify your email by opening this link:\n{link}\n\nIf you didn't request this, you can ignore this message.";

    private static string BuildForgotPasswordTextBody(string name, string resetLink) => $@"
        Hi {name},

        We received a request to reset your password.

        Reset your password by clicking this link:
        {resetLink}

        SECURITY NOTICE:
        - This link expires in 5 minutes
        - If you didn't request this reset, please ignore this email
        - Never share this link with anyone

        If the link doesn't work, copy and paste this URL into your browser:
        {resetLink}

        This is an automated message. Please do not reply to this email.
        If you need help, contact our support team.";
}