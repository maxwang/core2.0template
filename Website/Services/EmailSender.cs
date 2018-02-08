using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Website.Models;

namespace Website.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private SmtpSettings _options;
        public EmailSender(IOptions<SmtpSettings> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_options.NormalMailFrom));

            emailMessage.To.Add(new MailboxAddress(string.IsNullOrEmpty(email) ? _options.NormalMailTo : email));


            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                var credentials = new NetworkCredential
                {
                    UserName = _options.EmailServerUsername,
                    Password = _options.EmailServerPassword
                };

                //client.LocalDomain = _options.LocalDomain;
                client.Timeout = 120000;
                await client.ConnectAsync(_options.EmailHost, int.Parse(_options.EmailPort), SecureSocketOptions.Auto).ConfigureAwait(false);

                if (_options.EmailHost.ToUpper().Equals("SMTP.OFFICE365.COM"))
                {
                    client.AuthenticationMechanisms.Remove("PLAIN");
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                }
                await client.AuthenticateAsync(credentials);
                try
                {
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {

                }

                await client.DisconnectAsync(true).ConfigureAwait(false);

            }
        }
    }
}
