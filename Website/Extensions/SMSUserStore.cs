using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Website.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Data;

namespace Website.Extensions
{
    

    public class SMSUserStore<TUser> : UserStore<TUser>
        where TUser : ApplicationUser, new()
    {
        public SMSUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            
        }
        

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var aContext = Context as ApplicationDbContext;
            var results = from ur in aContext.UserRoles
                          join r in aContext.Roles on ur.RoleId equals r.Id
                          where ur.UserId == userId
                          select r.Name;

            return await results.ToListAsync(); 
        }

        public async Task<bool> IsInternalUser(string userId)
        {
            var aContext = Context as ApplicationDbContext;

            return await aContext.Roles.Join(aContext.UserRoles,
                            r => r.Id,
                            ur => ur.RoleId,
                            (r, ur) => new { Role = r, UserId = ur.UserId })
                            .AnyAsync(x => x.Role.IsInternal && x.UserId == userId);

            //return await aContext.Roles.AnyAsync(x => x.IsInternal && x.Users.Any(u => u.UserId == userId));
        }

        public Company GetCompany(int companyId)
        {
            var aContext = Context as ApplicationDbContext;
            return aContext.Companies.FirstOrDefault(x => x.Id == companyId);
        }

        public async Task<Company> GetCompanyAsync(int companyId)
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
        }

        public Company GetCompanyByZohoUuid(string zohoAccountUuid)
        {
            var aContext = Context as ApplicationDbContext;
            return aContext.Companies.FirstOrDefault(x => x.CompanyZohoAccountId == zohoAccountUuid);
        }

        public async Task<Company> GetCompanyByZohoUuidAsync(string zohoAccountUuid)
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.Companies.FirstOrDefaultAsync(x => x.CompanyZohoAccountId == zohoAccountUuid);
        }

        public IEnumerable<CompanyClaims> GetCompanyClaims(int companyId)
        {
            var aContext = Context as ApplicationDbContext;
            return aContext.CompanyClaims.Where(x => x.CompanyId == companyId).ToList();
        }

        public UserZohoContact GetUserZohoContact(string userId)
        {
            var aContext = Context as ApplicationDbContext;
            return aContext.UserZohoContacts.FirstOrDefault(x => x.UserId == userId);
        }

        public async Task<UserZohoContact> GetUserZohoContactAsync(string userId)
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.UserZohoContacts.FirstOrDefaultAsync(x => x.UserId  == userId);
        }

        public async Task<bool> CheckifACLEnabled(string zohoContactUUID)
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.UserZohoContacts.AsNoTracking().AnyAsync(x => x.ZohoContactId == zohoContactUUID);
        }

        public async Task<bool> IsInRoleAsync(string userId, string roleName)
        {
            var aContext = Context as ApplicationDbContext;
            var role = await aContext.Roles.FirstOrDefaultAsync(
                x => x.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));

            if (role != null)
            {
                return await aContext.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == role.Id);
            }

            return false;

        }
        

        public async Task<int> CreateUserZohoContactAsync(UserZohoContact userContact)
        {
            var aContext = Context as ApplicationDbContext;
            await aContext.UserZohoContacts.AddAsync(userContact);
            await aContext.SaveChangesAsync();
            return userContact.Id;
        }

        public async Task<IEnumerable<CompanyClaims>> GetCompanyClaimsAsync(int companyId)
        {
            var aContext = Context as ApplicationDbContext;
            return await aContext.CompanyClaims.Where(x => x.CompanyId == companyId).ToListAsync();
        }


        public async Task<bool> AnyCompanyExternalAdminAsync(int companyId)
        {
            var aContext = Context as ApplicationDbContext;
            var users = await aContext.Users.Where(x => x.CompanyId.Equals(companyId)).ToListAsync();
            foreach(var user in users)
            {
                if(await aContext.UserRoles.AnyAsync(ur => ur.UserId.Equals(user.Id) && ur.RoleId.Equals("EAdmin")))
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyCompanyExternalAdmin(int companyId)
        {
            var aContext = Context as ApplicationDbContext;
            var users = aContext.Users.Where(x => x.CompanyId.Equals(companyId)).ToList();
            foreach (var user in users)
            {
                if (aContext.UserRoles.Any(ur => ur.UserId.Equals(user.Id) && ur.RoleId.Equals("EAdmin")))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roleNames)
        {
            var aContext = Context as ApplicationDbContext;
            foreach (var roleName in roleNames)
            {
                var role = await aContext.Roles.FirstOrDefaultAsync(x => x.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
                if (role != null)
                {
                    var userRole = await aContext.UserRoles.FirstOrDefaultAsync( x => x.UserId == user.Id && x.RoleId == role.Id);
                    if (userRole != null)
                    {
                        aContext.UserRoles.Remove(userRole);
                    }
                }
                
            }

            await aContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roleNames)
        {
            var aContext = Context as ApplicationDbContext;
            foreach (var roleName in roleNames)
            {
                var role = await aContext.Roles.SingleOrDefaultAsync(x => x.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
                var result = await aContext.UserRoles.AddAsync(new IdentityUserRole<string> { RoleId = role.Id, UserId = user.Id });

            }
            
            await aContext.SaveChangesAsync();
            return IdentityResult.Success;

        }
    }
}
