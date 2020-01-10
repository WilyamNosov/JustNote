using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private ConfirmEmailService _emailService = new ConfirmEmailService();
        private IDatabaseItemService<User> _userService;

        public RegisterController(IDatabaseItemService<User> userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ConfirmEmail(string id)
        {
            try
            {
                var result = await _emailService.AcceptConfirmMessage(id);
                return Redirect(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser([FromBody] User user)
        {
            try
            {
                var hashKey = new HashKeyService().GetHashKey(user.HashKey);
                user.HashKey = hashKey;
                
                var callbackUrl = @"https://cb5eza7o22.execute-api.us-west-2.amazonaws.com/Prod/api/Register/" + new TokenManagerService().GenerateConfirmEmailToken(user.Email);
                var confirmEmailFormString = "";

                var page = @"https://justnoteservices3bucket.s3-us-west-2.amazonaws.com/ConfirmEmailForm.html";

                var webRequest = WebRequest.Create(page);

                var response = webRequest.GetResponse();
                var content = response.GetResponseStream();
                using (var reader = new System.IO.StreamReader(content))
                {
                    confirmEmailFormString = reader.ReadToEnd();
                }

                var outMessage = confirmEmailFormString.Split('|')[0] + user.Email.ToString();
                foreach (var partOfMessage in confirmEmailFormString.Split('|')[1].Split('@'))
                {
                    outMessage += partOfMessage + callbackUrl;
                }

                await _emailService.SendConfirmMessage(user.Email, "Confirm you account", outMessage.Substring(0, outMessage.Length - callbackUrl.Length));

                await _userService.Create(user);

                return Ok();

            } catch
            {
                return BadRequest();
            }
        }
    }
}
