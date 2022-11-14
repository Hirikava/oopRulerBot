using System.Net.Mail;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification.SmtpTransport;

public class SmtpVerificationTransport: ISmtpTransport
{
    
    private readonly ILog log;
    private readonly SmtpClient smtpClient;

    public SmtpVerificationTransport(ILog log, SmtpClient smtpClient)
    {
        this.log = log;
        this.smtpClient = smtpClient;

    }
    
    public async Task<bool> SendVerificationCode(string email, int code)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));
        try
        {
            await smtpClient.SendMailAsync("noreply@ooprulerbot.ru", email, "Verification code", code.ToString());
            return true;
        }
        catch (Exception e)
        {
            log.Error(e);
            return false;
        }
        
    }
}