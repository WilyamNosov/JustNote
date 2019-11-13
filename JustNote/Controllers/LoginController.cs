using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;

namespace JustNote.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> LoginUser(Login loginObject)
        {
            var userName = loginObject.UserName;
            var password = loginObject.UserPassword;
            
            var user = await new UserService().GetUser(userName, new HashKeyService().GetHashKey(password));
            
            if (user != null && user.ConfirmedEmail == true)
            {
                var token = new TokenManagerService().GenerateToken(userName, user.HashKey);
                return Ok(token);
            }
             
            return Unauthorized();
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> ValidateUser(string id)
        {
            var userName = "";
            var userHashKey = "";
            var tokenManagerService = new TokenManagerService();

            if (tokenManagerService.ValidateToken(id, out userName, out userHashKey))
            {
                return Ok(id);
            }

            return Unauthorized();
        }
    }
}