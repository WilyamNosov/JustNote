using System;
using System.Collections.Generic;
using System.Linq;
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
                //string callbackUrl = @"http://localhost:58228/api/Register/" + new TokenManagerService().GenerateConfirmEmailToken(user.Email);

                var callbackUrl = @"https://cb5eza7o22.execute-api.us-west-2.amazonaws.com/Prod/api/Register/" + new TokenManagerService().GenerateConfirmEmailToken(user.Email);

                await emailService.SendConfirmMessage(user.Email, "Confirm you account",
                    @"
                    <html>
                        <head>
                            <style>
        	                    *{
        		                    margin: 0;
        		                    padding: 0;
                                    color: #000;
        	                    }
                                .main-container {
                                    margin: 0 10%;
            	                    padding: 0 40px;
			                        background: #dfdfdf;
                                }
                                .container-body {
            	                    width: 60%;
            	                    background: rgb(255, 255, 255);
            	                    margin: 0 auto;
                                    box-shadow: 0px 0px 15px 10px rgba(0, 0, 0, 0.55);
                                }
                                .logo-header {
			                        padding: 15px;
			                    }
                                .logo-text {
            	                    font-family: Tangerine;
            	                    text-align: center;
            	                    font-size: 60px;
                                }
                                .verify-account {
			                        background: rgb(255, 165, 0);
			                        padding: 20px 0;
			                    }
			                    img.img-container {
			                        margin: 0 auto;
			                        display: block;
			                        width: 84px;
			                    }
			                    .verify-text {
			                        text-align: center;
			                        padding-top: 32px;
			                        font-size: 30px;
			                    }
			                    .content {
			                        padding: 0 35px;
			                    }
			                    .content-element{
			                        padding: 25px 0;
			                    }
			                    .account-information {
			                        border-bottom: 1px solid #000;
			                    }
			                    .account-information-head {
			                        font-size: 18px;
			                        font-weight: 600;
			                    }
			                    .account-information-content {
			                        margin-top: 20px;
			                    }
			                    .left-colum {
			                        font-size: 14px;
                                    font-weight: 600;
                                    vertical-align: top;
                                    width: 20%;
                                    float: left;
			                    }
			                    .right-colum {
			                        font-size: 14px;
                                    width: 80%;
                                    float: right;
			                    }
                                .button-container a {
                                    text-decoration: none;
                                }
			                    .button-style {
			                        display: block;
			                        margin: 0 auto;
			                        width: 45%;
			                        padding: 10px 0;
			                        font-size: 26px;
			                        background: rgb(255, 165, 0);
			                        color: rgb(255,255,255);
			                        border: 0;
			                        border-radius: 5px;
			                    }
                                .button-style:hover {
                                    background: rgb(225, 135, 0);
                                }
			                    .replying-text {
			                        font-size: 18px;
			                    }
			                    .footer {
			                        padding: 20px 35px;
                                    background: #afafaf;
			                    }
                                .thanks {
                                    padding-top: 10px;
                                }
                                @media screen and (max-width: 767px) {
                                    .main-container {
                                        margin: 0;
            	                        padding: 0 10px;
                                    }
                                    .container-body {
            	                        width: 100%;
                                    }
                                    .logo-text {
            	                        font-size: 40px;
                                    }
			                        img.img-container {
			                            width: 56px;
			                        }
			                        .verify-text {
			                            padding-top: 16px;
			                        }
			                        .account-information {
			                            border-bottom: none;
			                        }
			                        .account-information-head {
			                            font-size: 16px;
			                        }
                                    .my-flex {
                                        display: flex;
                                        flex-direction: column;
                                    }
			                        .left-colum {
                                        width: 100%;
			                        }
			                        .right-colum {
                                        padding-left: 0;
                                        margin-bottom: 10px;
                                        width: 100%;
			                        }
			                        .button-style {
			                            margin: 0 auto;
			                            width: 95%;
			                            font-size: 24px;
			                        }
			                        .replying-text {
			                            font-size: 16px;
			                        }
			                        .button-style {
			                            margin-top: 16px;
			                            font-size: 22px;
			                        }
			                        .content {
			                            padding: 0 15px;
			                        }
			                        .footer {
			                            padding: 20px 15px;
                                        background: #afafaf;
			                        }
                                }
                            </style>
                            <link href='https://fonts.googleapis.com/css?family=Tangerine' rel='stylesheet'>
                        </head>
                        <body>
                            <div class='main-container'>
        	                    <div class='container-body'>
        		
	        	                    <div class='logo-header'>
	        		                    <div class='logo-text'>Just Notes</div>
	        	                    </div>

	        	                    <div class='verify-account'>
	        		                    <img class='img-container' src='https://img.icons8.com/android/452/ok.png'>
	        		                    <div class='verify-text'>Verify You Account</div>
	        	                    </div>

	        	                    <div class='content'>
	        		
	        		                    <div class='content-element account-information'>
	        			                    <div class='account-information-head'>You account information</div>
	        			                    <div class='account-information-content'>
	        						            <div>
	        							            <div class='left-colum'>Account</div>
	        							            <div class='right-colum'>" + newUser.Email.ToString() + @"</div>
	        						            </div>
	        						            <div>
	        							            <div class='left-colum'>Verify link</div>
	        							            <div class='right-colum'><a href='" + callbackUrl + @"'>" + callbackUrl + @"</div>
	        						            </div>
	        			                    </div>
	        		                    </div>

	        		                    <div class='content-element button-container'>
	        			                    <a href='" + callbackUrl + @"'>
	        				                    <button class='button-style'>Verify You Account</button>
	        			                    </a>
	        		                    </div>

	        		                    <div class='content-element replying-text'>
	        			                    If you are having any issues with your account, please don`t hesitate to contact us by replying to this mail.
	        			                    <br/><span class='thanks'>Thanks!</span>
	        		                    </div>

	        	                    </div>

	        	                    <div class='footer'>
	        		                    You`re receiving this email because you have an account in <span>Just Notes</span>. If you are not sure why you`re receiving this, please contact us.	
	        	                    </div>

        	                    </div>
                            </div>
                        </body>
                    </html>	   
                    ");

                return Ok();
            }

            return Unauthorized();
        }
    }
}
