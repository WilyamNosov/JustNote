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
        public string LoginUser(Login loginObject)
        {
            string userName = loginObject.UserName;
            string password = loginObject.UserPassword;
            LoginRetunForm retunForm = new LoginRetunForm();
            
            User user = new UserService().GetUser(userName, new HashKeyService().GetHashKey(password)).GetAwaiter().GetResult();
            if (user != null)
                return new TokenManagerService().GenerateToken(userName, user.HashKey);
             
            return null;
        }
    }
}