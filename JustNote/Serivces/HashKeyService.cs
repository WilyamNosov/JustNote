using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JustNote.Serivces
{
    public class HashKeyService
    {
        public string GetHashKey(string password)
        {
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

                return Convert.ToBase64String(hash);
            }
        }
    }
}
