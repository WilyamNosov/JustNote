<<<<<<< HEAD
﻿//using JustNote.Serivces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace JustNote.Attributes
//{
//    public class JustNotesAuthorizeAttribute : AuthorizeAttribute
//    {
//        string outOne = "";
//        string outTwo = "";
//        //public void OnAuthorization(AuthorizationFilterContext context)
//        //{
//        //    var outOne = "";
//        //    var outTwo = "";

//        //    var token = context.HttpContext.Request.QueryString.Value.Split('=')[1];
//        //    var tokenManager = new TokenManagerService();

//        //    if (tokenManager.ValidateToken(token, out outOne, out outTwo))
//        //    {

//        //        return;
//        //    }
//        //}

//        const string POLICY_PREFIX = "MinimumAge";

//        public JustNotesAuthorizeAttribute(int age) => Age = age;
//        //public JustNotesAuthorizeAttribute(string token) => Verify = new TokenManagerService().ValidateToken(token, out outOne, out outTwo);

//        // Get or set the Age property by manipulating the underlying Policy property
//        public int Age
//        {
//            get
//            {
//                if (int.TryParse(Policy.Substring(POLICY_PREFIX.Length), out var age))
//                {
//                    return age;
//                }
//                return default(int);
//            }
//            set
//            {
//                Policy = $"{POLICY_PREFIX}{value.ToString()}";
//            }
//        }
//    }
//}
=======
﻿
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
            var x = context.HttpContext.Request.Query.Keys;
            context.HttpContext.Request.Query.TryGetValue("token", out token);
            var hasClaim = _tokenManagerService.ValidateToken(token);
            
            if (!hasClaim)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
>>>>>>> DatabaseData
