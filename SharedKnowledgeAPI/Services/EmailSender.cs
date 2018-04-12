using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute("DgdmLfRgTBejrSdcGqsKqg", subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            //var client = new SendGridClient(apiKey);
            //var msg = new SendGridMessage()
            //{
            //    From = new EmailAddress("sharedknowledge@thebest.com", "Edgar Zapeka"),
            //    Subject = subject,
            //    PlainTextContent = message,
            //    HtmlContent = message
            //};
            //msg.AddTo(new EmailAddress(email));
            //return client.SendEmailAsync(msg);
            MailMessage mailMsg = new MailMessage();

            // To
            mailMsg.To.Add(new MailAddress(email, "Dear user"));

            // From
            mailMsg.From = new MailAddress("sharedknowledge@thebest.com", "Edgar Zapeka");

            // Subject and multipart/alternative Body
            mailMsg.Subject = subject;
            string text = message;

            //mailMsg.AlternateViews.Add(
                    //AlternateView.CreateAlternateViewFromString(text,
                    //null, MediaTypeNames.Text.Plain));
            mailMsg.Body = message;

            // Init SmtpClient and send
            SmtpClient smtpClient
            = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials
            = new System.Net.NetworkCredential("EdgarZapekaBCIT",
                                               "Bcit1234!");
            smtpClient.Credentials = credentials;
            return smtpClient.SendMailAsync(mailMsg);
        }
    }
}
