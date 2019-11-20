﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustNote.Models;
using JustNote.Serivces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustNotes.Controllers
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
    }
}
