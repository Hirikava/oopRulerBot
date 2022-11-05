using System.Net.Mail;
using OopRulerBot.Verification.Transport;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification;

public class SmtpVerificationTransport : IVerificationTransport
{
    private readonly ILog log;
    private readonly SmtpClient client;
    private readonly MailAddress senderEmail;

    public SmtpVerificationTransport(ILog log, SmtpClient client, MailAddress senderEmail)
    {
        this.log = log.ForContext(nameof(SmtpVerificationTransport));
        this.client = client;
        this.senderEmail = senderEmail;
    }

    public async Task<bool> SendVerificationCode(string identifier, int code)
    {
        log.Info("Validating email");
        if (!MailAddress.TryCreate(identifier, out var targetEmail))
        {
            log.Info("Email is not valid");
            return false;
        }

        var message = new MailMessage(senderEmail, targetEmail)
        {
            Subject = "Verification code",
            Body = $"Your verification code is <b>{code}</b>",
            IsBodyHtml = true
        };

        log.Info("Email is valid. Sending verification code");
        try
        {
            await client.SendMailAsync(message);
            return true;
        }
        catch (Exception e)
        {
            log.Error(e);
            return false;
        }
    }
}