//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.Extensions.Options;

//namespace JustNote.Attributes.Authorization
//{
//    internal class JustNotePolicyProvider : IAuthorizationPolicyProvider
//    {
//        const string POLICY_PREFIX = "JustNote";
//        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

//        public JustNotePolicyProvider(IOptions<AuthorizationOptions> options)
//        {
//            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
//        }

//        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
//        {
//            return FallbackPolicyProvider.GetDefaultPolicyAsync();
//        }

//        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
//        {
//            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
//                int.TryParse(policyName.Substring(POLICY_PREFIX.Length), out var age))
//            {
//                var policy = new AuthorizationPolicyBuilder();
//                //policy.AddRequirements(new JustNoteRequirement(age));
//                return Task.FromResult(policy.Build());
//            }

//            return FallbackPolicyProvider.GetPolicyAsync(policyName);
//        }
//    }
//}
