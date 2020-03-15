using Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Services
{
    public class MailService: IMailService
    {
        public bool SendErrorEmail(UserEntity user)
        {
            MailMessage message = new MailMessage();
            message.Subject = "קרתה תקלה למשתמש :" + user.Id + " " + user.FirstName + " " + user.LastName;
            message.Body = "התקלה קרתה בזמן : " + DateTime.Now;
            return sendMail(message);
        }
        public bool SendAccessDeniedEmail(UserEntity user)
        {
            MailMessage message = new MailMessage();
            message.Subject = "נחסמה הגישה למשתמש :" + user.Id + " " + user.FirstName + " " + user.LastName;
            message.Body = "החסימה קרתה בזמן : " + DateTime.Now;
            return sendMail(message);
        }
        public bool SendSystemSabotage(UserEntity user,string reasonExplanation)
        {
            MailMessage message = new MailMessage();
            message.Subject = "המשתמש :" + user.Id + " " + user.FirstName + " " + user.LastName + "נחסם עקב התעסקות עם הגדרות המערכת ";
            message.Body = "החסימה קרתה בזמן : " + DateTime.Now + " " + reasonExplanation;
            return sendMail(message);
        }
        private bool sendMail(MailMessage message)
        {
            SmtpClient client = new SmtpClient();
            try
            {
                var Emailsettings = new SharedService().GetSystemSettings().GetSection("Email");
                client.Host = Emailsettings.GetSection("Host").Value;
                client.Port = Convert.ToInt32(Emailsettings.GetSection("Port").Value);
                client.EnableSsl = Convert.ToBoolean(Emailsettings.GetSection("EnableSsl").Value);
                client.UseDefaultCredentials = Convert.ToBoolean(Emailsettings.GetSection("UseDefaultCredentials").Value);
                client.Credentials = new NetworkCredential(Emailsettings.GetSection("Credentials:UserName").Value, Emailsettings.GetSection("Credentials:Password").Value);
                message.From = new MailAddress(Emailsettings.GetSection("MailFrom").Value);
                message.To.Add(Emailsettings.GetSection("MailTo").Value);
                client.Send(message);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            } finally
            {
                client.Dispose();
            }
        }
    }
}
