using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Principal;
using JustNote.Models;

namespace JustNote.Serivces
{
    public class TokenManagerService
    {
        private static string Secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";

        public string GenerateToken(string username, string hashkey)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim (ClaimTypes.Name, username),
                    new Claim (ClaimTypes.Hash, hashkey)
                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        public bool ValidateToken(string token, out string username, out string hashkey)
        {
            username = null;
            hashkey = null;

            ClaimsPrincipal simplePrinciple = GetPrincipal(token);

            try
            {
                ClaimsIdentity identity = simplePrinciple.Identity as ClaimsIdentity;

                if (identity == null)
                    return false;

                if (!identity.IsAuthenticated)
                    return false;

                Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
                Claim hashkeyClaim = identity.FindFirst(ClaimTypes.Hash);

                username = usernameClaim?.Value;
                hashkey = hashkeyClaim?.Value;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(hashkey))
                    return false;

                return true;

            } catch
            {
                return false;
            }
        }
        ///*protected */public Task<IPrincipal> AuthenticateJwtToken(string token)
        //{
        //    string username;

        //    if (ValidateToken(token, out username))
        //    {
        //        List<Claim> claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, username)
        //        };

        //        ClaimsIdentity identity = new ClaimsIdentity(claims, "Jwt");
        //        IPrincipal user = new ClaimsPrincipal(identity);

        //        return Task.FromResult(user);
        //    }

        //    return Task.FromResult<IPrincipal>(null);
        //}
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                byte[] symmetricKey = Convert.FromBase64String(Secret);

                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
