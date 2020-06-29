using Microsoft.AspNetCore.Hosting;
using MimeKit;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Services
{
    public class EmailSenderLogic : IEmailSenderLogic
    {
        private readonly IWebHostEnvironment _env; 
        public EmailSenderLogic(IWebHostEnvironment env)
        {
            _env = env;          
        }

        public async Task SendEmailAsync(Email emailData, SmtpConfig config)
        {
            try
            {
                var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot");
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(emailData.FromDisplayName, emailData.FromEmailAddress));
                email.To.Add(new MailboxAddress(emailData.ToName, emailData.ToEmailAddress));
                email.Subject = emailData.Subject;
                var content = string.Empty;

               /* var urlMail = Path.Combine(mainUrl, "emailResources", "emailTemplate.html");
               
                var objReader = new StreamReader(urlMail);
                content = objReader.ReadToEnd();
                objReader.Close();

                content = Regex.Replace(content, "{BoydText}", emailData.Message);*/

                var body = new BodyBuilder
                {
                    HtmlBody = emailData.Message
                };

                if (emailData.Array != null)
                {
                    body.Attachments.Add("Proposal.pdf", emailData.Array, new ContentType("application", "pdf"));
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
