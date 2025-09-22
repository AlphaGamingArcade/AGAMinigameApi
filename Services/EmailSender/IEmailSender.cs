namespace AGAMinigameApi.Services.EmailSender
{
    public interface IEmailSender
    {
        Task SendVerificationEmailAsync(string toEmail, string displayName, string verificationLink);
        Task SendForgotPasswordAsync(string toEmail, string displayName, string forgotPasswordLink);
    }

}