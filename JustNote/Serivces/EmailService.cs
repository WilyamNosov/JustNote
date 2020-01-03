using System;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;

namespace JustNote.Serivces
{
    public class EmailService
    {
        public EmailService()
        {
            var x = SendSimpleMessage().Content.ToString();
        }

        public static IRestResponse SendSimpleMessage()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");

            client.Authenticator =

            new HttpBasicAuthenticator("api",
                "439856d912c64fa6659091a365f8663a-6f4beb0a-b7065b99");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandboxe29a95d0c9b14cd8b8405ba485563461.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <mailgun@sandboxe29a95d0c9b14cd8b8405ba485563461.mailgun.org>");
            request.AddParameter("to", "justnotemain@gmail.com");
            request.AddParameter("to", "YOU@sandboxe29a95d0c9b14cd8b8405ba485563461.mailgun.org");
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }
}
