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
        private static string _secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";
        public string UserName { get; set; }
        public string UserHashKey { get; set; }

        public string GenerateToken(string username, string hashkey)
        {
            byte[] key = Convert.FromBase64String(_secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim (ClaimTypes.Name, username),
                    new Claim (ClaimTypes.Hash, hashkey)
                }),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        public bool ValidateToken(string token/*, out string username, out string hashkey*/)
        {
            UserName = null;
            UserHashKey = null;

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

                UserName = usernameClaim?.Value;
                UserHashKey = hashkeyClaim?.Value;

                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserHashKey))
                    return false;

                return true;

            }
            catch
            {
                return false;
            }
        }
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                byte[] symmetricKey = Convert.FromBase64String(_secret);

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

        public string GenerateConfirmEmailToken(string userEmail)
        {
            byte[] key = Convert.FromBase64String(_secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim (ClaimTypes.Email, userEmail)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        public bool ValidateConfirmEmailToken(string token, out string email)
        {
            email = null;

            ClaimsPrincipal simplePrinciple = GetPrincipal(token);

            try
            {
                ClaimsIdentity identity = simplePrinciple.Identity as ClaimsIdentity;

                if (identity == null)
                    return false;

                if (!identity.IsAuthenticated)
                    return false;

                Claim emailClaim = identity.FindFirst(ClaimTypes.Email);

                email = emailClaim?.Value;

                if (string.IsNullOrEmpty(email))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
