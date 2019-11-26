using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JustNotes.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : Controller
    {
        IDatabaseItemService<User> _userService;

        public LoginController(IDatabaseItemService<User> userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(Object inputValue)
        {
            var userName = JObject.Parse(inputValue.ToString()).Value<String>("UserName");
            var password = JObject.Parse(inputValue.ToString()).Value<String>("UserPassword");
            var hashKey = new HashKeyService().GetHashKey(password);

            try
            {
                var user = await new UserService().GetUser(userName, hashKey);
                var token = new TokenManagerService().GenerateToken(userName, user.HashKey);

                return Ok(token);
            } catch
            {
                return Unauthorized();
            }
        }
    }
}
