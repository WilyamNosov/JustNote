//using Microsoft.AspNetCore.Authorization;

//namespace JustNote.Attributes.Authorization
//{
//    internal class JustNoteAuthorizeAttribute : AuthorizeAttribute
//    {
//        const string POLICY_PREFIX = "MinimumAge";

//        public JustNoteAuthorizeAttribute(string token) => Verify = new TokenManagerService();

//        // Get or set the Age property by manipulating the underlying Policy property
//        public bool Verify
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
