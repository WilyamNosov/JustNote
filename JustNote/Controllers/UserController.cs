﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;

namespace JustNote.Controllers
{
    [Route("api/User")]
    public class UserController : Controller
    {
        private string userName;
        private string userToken;
        [HttpPut("{id}")]
        public IActionResult UpdateUserData(string id, [FromBody]Object userData)
        {
            var tokenManager = new TokenManagerService();
            if (tokenManager.ValidateToken(id, out userName, out userToken))
            {
                AmazonS3Client client = new AmazonS3Client();
            }

            return Unauthorized();
        }
    }
}