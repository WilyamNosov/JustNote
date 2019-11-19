using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;

namespace JustNote.Serivces
{
    public class ConfirmEmailService
    {
        private UserService userService = new UserService();
        private string email;

        public async Task SendConfirmMessage(string email, string subject, string message)
        {
            MimeMessage emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("JustNote", "justnotemail@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (SmtpClient client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("justnotemain@gmail.com", "Pa$$w0Rd");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
        public async Task AcceptConfirmMessage(string token)
        {
            if (new TokenManagerService().ValidateConfirmEmailToken(token, out email))
            {
                User user = await userService.GetUserByEmail(email);
                if (user.ConfirmedEmail != true)
                {
                    user.ConfirmedEmail = true;

                    await userService.UpdateUser(user);
                    throw new Exception($"https://testawslambdas3bucket.s3.us-west-2.amazonaws.com/index.html");
                }
                else
                {
                    throw new Exception($"https://testawslambdas3bucket.s3.us-west-2.amazonaws.com/index.html");
                }
            }
        }
    }
}
