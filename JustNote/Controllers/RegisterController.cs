using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustNotes.Controllers
{
    [Route("api/Register")]
    [ApiController]
    public class RegisterController : Controller
    {
        private ConfirmEmailService emailService = new ConfirmEmailService();

        [HttpGet("{id}")]
        public async Task<IActionResult> ConfirmEmail(string id)
        {
            try
            {
                await emailService.AcceptConfirmMessage(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return Redirect(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser([FromBody] Registration newUser)
        {
            string hashKey = new HashKeyService().GetHashKey(newUser.Password);
            if (await new UserService().CreateUser(newUser, hashKey))
            {
                var user = new UserService().GetUserByEmail(newUser.Email).GetAwaiter().GetResult();
                
                var callbackUrl = @"https://cb5eza7o22.execute-api.us-west-2.amazonaws.com/Prod/api/Register/" + new TokenManagerService().GenerateConfirmEmailToken(user.Email);
                var confirmEmailFormString = "";

                using (System.IO.FileStream fs = System.IO.File.OpenRead("Pages/ConfirmEmailForm.html"))
                {
                    var byteArray = new byte[fs.Length];
                    fs.Read(byteArray, 0, byteArray.Length);
                    confirmEmailFormString = Encoding.Default.GetString(byteArray);
                }

                var outMessage = confirmEmailFormString.Split('|')[0] + newUser.Email.ToString();
                foreach (var partOfMessage in confirmEmailFormString.Split('|')[1].Split('@'))
                {
                    outMessage += partOfMessage + callbackUrl;
                }

                await emailService.SendConfirmMessage(user.Email, "Confirm you account", outMessage.Substring(0, outMessage.Length - callbackUrl.Length));

                return Ok();
            }

            return Unauthorized();
        }
    }
}
