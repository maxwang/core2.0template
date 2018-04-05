using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Extensions
{
    public class ClaimAuthorizePolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        private static object _myLock = new object();

        public ClaimAuthorizePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return await Task.FromResult(_options.DefaultPolicy);
        }

        public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy policy;

            lock (_myLock)
            {
                policy = _options.GetPolicy(policyName);
            }


            if (policy == null && !string.IsNullOrEmpty(policyName) && policyName.IndexOf("=====") > -1)
            {

                string[] resourceValues = policyName.Split(new string[] { "=====" }, StringSplitOptions.RemoveEmptyEntries);
                if (resourceValues.Length > 1)
                {
                    lock (_myLock)
                    {
                        _options.AddPolicy(policyName, builder =>
                        {
                            builder.RequireClaim(resourceValues[0], new string[] { resourceValues[1] });
                        });
                    }
                }
            }

            return await Task.FromResult(_options.GetPolicy(policyName));
        }
    }
}
