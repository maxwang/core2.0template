using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Website.Data;

namespace Website.Extensions
{
    public class SMSClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        
        public SMSClaimsPrincipalFactory(SMSUserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : 
            base(userManager, roleManager, optionsAccessor)
        {
            
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
           
            var principal = await base.CreateAsync(user);

            //var userId = await UserManager.GetUserIdAsync(user);

            var identity = (ClaimsIdentity) principal.Identity;
            var uManager = UserManager as SMSUserManager<ApplicationUser>;
            
            //Add Company Claims
            if(user.CompanyId.HasValue)
            {
                var claims = await uManager.GetCompanyClaimsAsync(user.CompanyId.Value);
                foreach(var c in claims)
                {
                    if (!identity.HasClaim(c.ClaimType, c.ClaimValue))
                    {
                        identity.AddClaim(new Claim(c.ClaimType, c.ClaimValue));
                    }
                }
            }
           
            return principal;
        }
    }
}
