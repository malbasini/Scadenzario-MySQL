using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MyCourse.Models.Options;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Scadenzario.Models.Options;

namespace Scadenzario.Models.Services.Infrastructure
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly IOptionsMonitor<SmtpOptions> smtpOptionsMonitor;
        private readonly ILogger<MailKitEmailSender> logger;
        private readonly IConfiguration configuration;
        public string MailFrom { get; set; }
      
        public MailKitEmailSender(IOptionsMonitor<SmtpOptions> smtpOptionsMonitor, ILogger<MailKitEmailSender> logger,IConfiguration configuration)
        {
            this.logger = logger;
            this.smtpOptionsMonitor = smtpOptionsMonitor;
            this.configuration = configuration;
        }
        public async Task SendEmailAsync(string recipientEmail, string subject, string htmlMessage)
        {
            try
            {
                var options = this.smtpOptionsMonitor.CurrentValue;

                String Host = configuration.GetValue<String>("Smtp:Host");
                String Username = configuration.GetValue<String>("Smtp:User");
                String Password = configuration.GetValue<String>("Smtp:Password");

                using SmtpClient client = new();
                await client.ConnectAsync(Host, options.Port, options.Security);
                if (!string.IsNullOrEmpty(Username))
                {
                    await client.AuthenticateAsync(Username, Password);
                }

                MimeMessage message = new();
                if(string.IsNullOrEmpty(MailFrom))
                    message.From.Add(MailboxAddress.Parse(options.Sender));
                else
                    message.From.Add(MailboxAddress.Parse(MailFrom));
                message.To.Add(MailboxAddress.Parse(recipientEmail));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Couldn't send email to {recipientEmail} with message {message}", recipientEmail, htmlMessage);
            }

        }

       
    }
}
