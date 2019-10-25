using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;

namespace JustNote.Controllers
{
    [Route("api/Register")]
    [ApiController]
    public class RegisterController : Controller
    {
        [HttpPost]
        public IActionResult CreateNewUser([FromBody] Registration newUser)
        {
            string hashKey = new HashKeyService().GetHashKey(newUser.Password);
            if (new UserService().CreateUser(newUser, hashKey).GetAwaiter().GetResult())
                return Ok();
            else
                return new UnauthorizedResult();
        }
    }
}