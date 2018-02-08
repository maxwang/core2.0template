using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Website.Models;
using Microsoft.AspNetCore.Authentication;

namespace Website.Extensions
{
    
    public class SMSSignManager: SignInManager<ApplicationUser>
    {
        public SMSSignManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null || user.IsActive == false)
            {
                return SignInResult.Failed;
            }
            
            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            
        }

        public override Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override Task RefreshSignInAsync(ApplicationUser user)
        {
            return base.RefreshSignInAsync(user);
        }

        //this is for remember me login
        public override async Task<ApplicationUser> ValidateSecurityStampAsync(ClaimsPrincipal principal)
        {
            var result = await base.ValidateSecurityStampAsync(principal);

            if (result != null && result.IsActive)
            {
                result.LastLoginDate = DateTime.Now;
                await UserManager.UpdateAsync(result);
            }

            return result != null && result.IsActive ? result : null;
        }
    }
}
