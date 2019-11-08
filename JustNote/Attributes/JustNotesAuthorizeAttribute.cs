using JustNote.Serivces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustNote.Attributes
{
    public class JustNotesAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var outOne = "";
            var outTwo = "";

            var token = context.HttpContext.Request.QueryString.Value.Split('=')[1];
            var tokenManager = new TokenManagerService();

            if (tokenManager.ValidateToken(token, out outOne, out outTwo))
            {
                
                return;
            }
        }
    }
}
