using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace JustNote.Serivces
{
    public class EmailService
    {
        private ConfirmEmailService _emailService = new ConfirmEmailService();

        public EmailService()
        {

        }
        public async Task<bool> ShareItemMessageBuild(string senderEmail, string getterEmail, string itemId)
        {
            var callbackUrl = @"http://localhost:3000/?id=" + itemId;
            var confirmEmailFormString = "";

            var page = @"https://justnoteservices3bucket.s3-us-west-2.amazonaws.com/ShareNoteForm.html";

            var webRequest = WebRequest.Create(page);

            var response = webRequest.GetResponse();
            var content = response.GetResponseStream();
            using (var reader = new System.IO.StreamReader(content))
            {
                confirmEmailFormString = reader.ReadToEnd();
            }

            var outMessage = confirmEmailFormString.Split('|')[0] + senderEmail;
            foreach (var partOfMessage in confirmEmailFormString.Split('|')[1].Split('@'))
            {
                outMessage += partOfMessage + callbackUrl;
            }

            await _emailService.SendConfirmMessage(getterEmail, "Share note", outMessage.Substring(0, outMessage.Length - callbackUrl.Length));

            return true;
        }
    }
}
