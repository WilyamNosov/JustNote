using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace JustNote.Controllers
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
            } catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewUser([FromBody] Registration newUser)
        {
            string hashKey = new HashKeyService().GetHashKey(newUser.Password);
            if (new UserService().CreateUser(newUser, hashKey).GetAwaiter().GetResult())
            {
                User user = new UserService().GetUserByEmail(newUser.Email).GetAwaiter().GetResult();

                string callbackUrl = @"http://localhost:58228/api/Register/" + new TokenManagerService().GenerateConfirmEmailToken(user.Email);

                await emailService.SendConfirmMessage(user.Email, "Confirm your account",
                    $"Подтвердите регистрацию, перейдя по ссылке: " +
                    $"<a href='{callbackUrl}'>link</a>");

                return Ok();
            }

            return Unauthorized();
        }
    }
}