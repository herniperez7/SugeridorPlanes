using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly IWebHostEnvironment _env;
        public IConfiguration _configuration { get; }
        public EmailSender(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string fromDisplayName, string fromEmailAddress, string toName,
            string toEmailAddress, string subject, string message, byte[] array)
        {
            try
            {
    
               // var test = _configuration.GetSection("JsonTest").GetSection("NestedJson").Value;

                var userName = _configuration.GetSection("EmailConfiguration").GetSection("UserName").Value;
                var password = _configuration.GetSection("EmailConfiguration").GetSection("Password").Value;
                var smtpHost = _configuration.GetSection("EmailConfiguration").GetSection("SmtpHost").Value;
                var portTest = _configuration.GetSection("EmailConfiguration").GetSection("Port").Value;
                // var port = int.Parse(_configuration.GetSection("EmailConfiguration").GetSection("Port").Value);
                var port = 587;

                var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot", "html");
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(fromDisplayName, fromEmailAddress));
                email.To.Add(new MailboxAddress(toName, toEmailAddress));
                email.Subject = subject;

                var body = new BodyBuilder
                {
                    HtmlBody = message
                };

                if (array != null)
                {
                    body.Attachments.Add("Propuesta comercial", array, new ContentType("application", "pdf"));
                }                

                email.Body = body.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Start of provider specific settings
                    await client.ConnectAsync(smtpHost, port, false).ConfigureAwait(false);
                    await client.AuthenticateAsync(userName, password).ConfigureAwait(false);
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
