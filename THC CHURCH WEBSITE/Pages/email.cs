using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace THC_CHURCH_WEBSITE.Pages
{

    public class EmailSenders : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.office365.com",
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                
                Credentials = new NetworkCredential("roughshodnewt@outlook.com", "lxwiisyarskongmj")
            };

            return client.SendMailAsync("roughshodnewt@outlook.com", email, subject, htmlMessage);
        }
    }
}
