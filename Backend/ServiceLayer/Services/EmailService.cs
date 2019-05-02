using System;
using MimeKit;
using MailKit.Net.Smtp;

namespace ServiceLayer.Services
{
    public class EmailService: IEmailService
    {
        private string SmtpServer = "email-smtp.us-west-2.amazonaws.com";
        private int SmtpPort = 25;
        private string SmtpUsername = "AKIAIYJ66B6UXRL6LKFQ";
        private string SmtpPassword = "BEA+vRI2ZS+i4P1c2Atma1VZhIIKwh8eaS7Gx6EmMx8k";

        //Function to send an email without formatting
        public MimeMessage CreateEmailPlainBody(string receiverName, string receiverEmail, string emailSubject, string emailBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Support", "no-reply@kfc-sso.com"));
            message.To.Add(new MailboxAddress(receiverName, receiverEmail));
            message.Subject = emailSubject;
            message.Body = new TextPart("plain")
            {
                Text = emailBody
            };
            return message;
        }

        public void SendEmail(MimeMessage messageToSend)
        {
            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect(SmtpServer, SmtpPort); 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(SmtpUsername, SmtpPassword);
                emailClient.Send(messageToSend);
                emailClient.Disconnect(true);
            }
        }
    }
}
