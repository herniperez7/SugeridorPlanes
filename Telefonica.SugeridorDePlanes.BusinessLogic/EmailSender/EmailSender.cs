using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.EmailSender
{
    public class EmailSender
    {
        public static void Email(string htmlString)
        {
            
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("perez.hernan1996@gmail.com");
                message.To.Add(new MailAddress("perez.hernan1996@gmail.com"));
                message.Subject = "Test";
                //message.Attachments.Add(new Attachment());
                message.IsBodyHtml = false; //to make message body as html  
                message.Body = htmlString;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("perez.hernan1996@gmail.com", "4986585-1h");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                smtp.Send(message);
            }
            catch (Exception)
            {

            }
                
            
                           
            
        }
    }
}
