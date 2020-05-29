using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly IWebHostEnvironment _env; 
        public EmailSender(IWebHostEnvironment env)
        {
            _env = env;          
        }

        public async Task SendEmailAsync(Email emailData, SmtpConfig config)
        {
            try
            {
                var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot", "html");
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(emailData.FromDisplayName, emailData.FromEmailAddress));
                email.To.Add(new MailboxAddress(emailData.ToName, emailData.ToEmailAddress));
                email.Subject = emailData.Subject;

                var body = new BodyBuilder
                {
                    HtmlBody = emailData.Message
                };

                if (emailData.Array != null)
                {
                    body.Attachments.Add("Propuesta comercial", emailData.Array, new ContentType("application", "pdf"));
                }

                email.Body = body.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Start of provider specific settings
                    await client.ConnectAsync(config.SmtpHost, config.Port, false).ConfigureAwait(false);
                    await client.AuthenticateAsync(config.UserName, config.Password).ConfigureAwait(false);
                    // End of provider specific settings

                    await client.SendAsync(email).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
