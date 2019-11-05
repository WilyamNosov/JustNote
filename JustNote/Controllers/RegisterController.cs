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
                //string callbackUrl = @"http://localhost:58228/api/Register/" + new TokenManagerService().GenerateConfirmEmailToken(user.Email);

                string callbackUrl = @"https://cb5eza7o22.execute-api.us-west-2.amazonaws.com/Prod/api/Register/" + new TokenManagerService().GenerateConfirmEmailToken(user.Email);

                await emailService.SendConfirmMessage(user.Email, "Confirm you account",

                    @"
                <html>
                    <head>
                        <style>
                            .container {
                                padding: 40px; 
                                width: 60%; 
                                max-width: 600px; 
                                background-color: rgb(255, 255, 255);
                                border-left-color: rgb(93, 158, 163); 
                                border-right-color: rgb(93, 158, 163); 
                                margin: 0 auto; 
                                display: grid; 
                                border-radius: 12px; 
                                box-shadow: 0px 0px 12px 6px rgba(0,0,0, 0.45);
                                text-align: center;
                            }
                            .justnote {
                                color: #007939;
                            }
                            .button-container {
                                display: inline-block; 
                                margin: 0 auto;
                            }
                            .button-class {
                                width: 250px; 
                                height: 50px; 
                                background: #00b349; 
                                border: 0; border-radius: 10px; 
                                font-size: 22px; 
                                color: rgb(255, 255, 255); 
                                font-weight: normal; cursor: pointer;
                            }
                            @media screen and (max-width: 767px) {
                                .container {
                                    padding: 20px; 
                                }
                                .button-class {
                                    width: 100%;
                                    padding: 0px 40px;
                                }
                                .main-text h1 {
                                    font-size: 1.25em;
                                }
                                .main-text h3 {
                                    font-size: 1em;
                                }
                            }
                        </style>
                    </head>
                    <body>
                        <div style='background-color: rgb(0, 61, 52); background-repeat: repeat-x; width: 100%; margin: 0 auto;'>
		                    <div current-email-width='600' style='padding-top:20px; padding-bottom:20px;'>
			                    <div>

				                    <div>
					                    <div>
						                    <span></span>
					                    </div>
				                    </div>

				                    <div class='container main-text'>
					                    
								                    <h1>Confirm your account</h1>
								                    <h3>Thank you for registering in the application <span class='justnote'>JustNote</span>!<br/>
								                    To confirm your email address, click on the button below..</h3>
					                    
					                    <div class='button-container'>
						                    <a href='" + callbackUrl + @"' style='display: block;'>
							                    <button class='button-class' >Confirm</button>
                                            </a>
					                    </div>
				                    </div>

				                    <div>
					                    <div>
						                    <span></span>
					                    </div>
				                    </div>

			                    </div>
		                    </div>
	                    </div>
                    </body>
                    </html>");

                /*await emailService.SendConfirmMessage(user.Email, "Confirm your account",
                    $"Подтвердите регистрацию, перейдя по ссылке: " +
                    $"<a href='{callbackUrl}'>link</a>");*/

                return Ok();
            }

            return Unauthorized();
        }
    }
}