using PruebaDesempenio.Infrastructure.Data;
using PruebaDesempenio.Models;

namespace PruebaDesempenio.Infrastructure.Services;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
public class EmailService
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public EmailService(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var log = new EmailLog
        {
            Recipient = toEmail,
        };
        try
        {
            var emailSettings = _config.GetSection("EmailSettings");

            using (var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"])))
            {
                client.Credentials = new NetworkCredential(
                    emailSettings["SenderEmail"],
                    emailSettings["SenderPassword"]
                );
                client.EnableSsl = true;

                var mail = new MailMessage
                {
                    From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                await client.SendMailAsync(mail);
                
                log.SentSuccesfully = true;
            }   
        }
        catch (Exception e)
        {
            log.SentSuccesfully = false;
            log.ErrorMessage = e.Message;
        }
        await _context.EmailLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}