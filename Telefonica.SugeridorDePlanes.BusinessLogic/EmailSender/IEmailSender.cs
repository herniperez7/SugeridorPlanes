using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(
            string fromDisplayName,
            string fromEmailAddress,
            string toName,
            string toEmailAddress,
            string subject,
            string message,
            byte[] array);
    }
}
