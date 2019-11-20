
using JustNote.Serivces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace JustNote.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JustNotesAuthorizeAttribute : TypeFilterAttribute
    {
        public JustNotesAuthorizeAttribute() : base(typeof(JustNotesAuthorizeFilter))
        {
        }
    }
    public class JustNotesAuthorizeFilter : IAuthorizationFilter
    {
        private StringValues token;
        private TokenManagerService _tokenManagerService;

        public JustNotesAuthorizeFilter(TokenManagerService tokenManagerService)
        {
            _tokenManagerService = tokenManagerService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.Query.TryGetValue("token", out token);
            var hasClaim = _tokenManagerService.ValidateToken(token);
            
            if (!hasClaim)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
