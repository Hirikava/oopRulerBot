using System.Net;
using System.Net.Mail;
using MimeKit;
using OopRulerBot.Settings;
using Vostok.Logging.Abstractions;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace OopRulerBot.Verification.Transport;

public sealed class SmtpVerificationTransport : IVerificationTransport, IDisposable
{
    private readonly ILog log;
    private readonly ThreadLocal<SmtpClient> client; // can't send messages in parallel, this should help prevent creating new client for *every* verification send
    private readonly MailAddress from;

    public SmtpVerificationTransport(ILog log, SmtpSettings settings)
    {
        this.log = log;
        client = new ThreadLocal<SmtpClient>(() => CreateClient(settings), true);
        from = new MailAddress(settings.SenderMailAddress);
    }

    private static SmtpClient CreateClient(SmtpSettings settings)
    {
        var client = new SmtpClient();
        client.Connect(settings.Host, settings.Port, true);
        client.Authenticate(settings.Username, settings.Password);

        return client;
    }

    public async Task<bool> SendVerificationCode(string identifier, int code)
    {
        using var message = new MailMessage
        {
            Body = $"<p>Your verification code is <b>{code}</b></p>", 
            IsBodyHtml = true, 
            Subject = "oopRulerBot verification code", 
            From = from,
        };
        message.To.Add(identifier);

        using var mimeMessage = MimeMessage.CreateFromMailMessage(message);
        try
        {
            var actualClient = client.Value;
            if (actualClient is null)
                return false;
            await actualClient.SendAsync(mimeMessage);
        }
        catch (Exception e)
        {
            log.Error(e, "Exception happend while sending verification");
            return false;
        }

        return true;
    }

    public string Name => "Mail";

    public void Dispose()
    {
        lock (client)
        {
            foreach (var smtpClient in client.Values)
            {
                smtpClient.Dispose();
            }
            client.Dispose();
        }
    }
}