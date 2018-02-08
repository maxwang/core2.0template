using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Website.Models;

namespace Website.Extensions
{
    public class SMSRoleManager : RoleManager<ApplicationRole>
    {
        public SMSRoleManager(IRoleStore<ApplicationRole> store, IEnumerable<IRoleValidator<ApplicationRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<ApplicationRole>> logger) 
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {

        }
        
        public async Task<IList<Resource>> GetModuleResoucesAsync()
        {
            var uStore = Store as SMSRoleStore;
            return await uStore.GetModuleResoucesAsync();
        }

        public async Task<IList<ApplicationRole>> GetExternalRolesAsync()
        {
            var uStore = Store as SMSRoleStore;
            return await uStore.GetExternalRolesAsync();
        }

        public async Task<IList<ApplicationRole>> GetInternalRolesAsync()
        {
            var uStore = Store as SMSRoleStore;
            return await uStore.GetInternalRolesAsync();
        }

        public async Task<bool> RoleHasClaimAsync(string roleId, string claimType, string claimValue)
        {
            var uStore = Store as SMSRoleStore;
            return await uStore.RoleHasClaimAsync(roleId, claimType, claimValue);   
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleId)
        {
            var uStore = Store as SMSRoleStore;
            return await uStore.GetUsersInRoleAsync(roleId);
        }
    }
}
