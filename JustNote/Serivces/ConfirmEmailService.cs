﻿using MimeKit;
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
        private UzverService _userService = new UzverService();
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
        }
    }
}
