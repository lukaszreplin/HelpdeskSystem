using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace HelpdeskSystem.Utils
{
    public class Mailing
    {
        public static void SendMail(string to, string subject, string body)
        {
            var messsage = new System.Net.Mail.MailMessage(ConfigurationManager.AppSettings["sender"], to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            var smtpClient = new System.Net.Mail.SmtpClient
            {

                Host = ConfigurationManager.AppSettings["smtpHost"],
                Credentials = new System.Net.NetworkCredential(
                    ConfigurationManager.AppSettings["sender"],
                    ConfigurationManager.AppSettings["password"]),
                EnableSsl = true
            };
            smtpClient.Send(messsage);
        }
    }
}