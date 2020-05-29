using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Email emailData, SmtpConfig config);
       
    }
}
