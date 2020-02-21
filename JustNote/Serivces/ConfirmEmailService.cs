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
<<<<<<< HEAD
        private UserService userService = new UserService();
=======
        private UzverService _userService = new UzverService();
>>>>>>> DatabaseData
        private string email;

        public async Task SendConfirmMessage(string email, string subject, string message)
        {
            MimeMessage emailMessage = new MimeMessage();

<<<<<<< HEAD
            emailMessage.From.Add(new MailboxAddress("JustNote", "justnotemail@gmail.com"));
=======
            emailMessage.From.Add(new MailboxAddress("JustNote", "justnotemain@gmail.com"));
>>>>>>> DatabaseData
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (SmtpClient client = new SmtpClient())
<<<<<<< HEAD
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
=======
            {   try
                {
                    await client.ConnectAsync("in-v3.mailjet.com", 465, true);
                    await client.AuthenticateAsync("0bff859eb124cb7864c89f4249f50926", "4b1fad7f175f3e02d89bd0472bbf8745");
                    await client.SendAsync(emailMessage);


                    await client.DisconnectAsync(true);
                } catch (Exception ex)
                {
                    var exception = ex.Message;
                }
            }
        }
        public async Task<string> AcceptConfirmMessage(string token)
        {
            if (new TokenManagerService().ValidateConfirmEmailToken(token, out email))
            {
                User user = await _userService.GetUserByEmail(email);
                
                if (user.ConfirmedEmail != true)
                {
                    user.ConfirmedEmail = true;
                    await _userService.Update(user.Id, user);
                    return $"https://testawslambdas3bucket.s3.us-west-2.amazonaws.com/index.html";
                }
            }

            throw new Exception("Bad request.");
>>>>>>> DatabaseData
        }
    }
}
