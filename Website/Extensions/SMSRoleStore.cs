using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Website.Data;
using Website.Models;

namespace Website.Extensions
{
    public class SMSRoleStore : RoleStore<ApplicationRole>
    {
        public SMSRoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public async Task<bool> RoleHasClaimAsync(string roleId, string claimType, string claimValue)
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.RoleClaims.AnyAsync(
                x => x.RoleId.Equals(roleId, StringComparison.CurrentCultureIgnoreCase) &&
                     x.ClaimType.Equals(claimType, StringComparison.CurrentCultureIgnoreCase) &&
                     x.ClaimValue.Equals(claimValue, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<IList<ApplicationRole>> GetExternalRolesAsync()
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.Roles
                .Where(x => x.IsInternal == false)
                .OrderBy(x => x.Name)
                .Select(x => x)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<ApplicationRole>> GetInternalRolesAsync()
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.Roles
                .Where(x => x.IsInternal == true)
                .OrderBy(x => x.Name)
                .Select(x => x)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<Resource>> GetModuleResoucesAsync()
        {
            var aContext = Context as ApplicationDbContext;
            var results = aContext.Resources
                            .AsNoTracking()
                            .Where(x => !string.IsNullOrEmpty(x.ModuleName))
                            .OrderBy(x => x.ModuleName)
                            .ThenBy(x=>x.ClaimType)
                            .Select(x => x);
                

            return await results.ToListAsync();
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleId)
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.Users.Where(x => x.Roles.Any(r => r.RoleId.Equals(roleId))).ToListAsync();
        }


    }
}
