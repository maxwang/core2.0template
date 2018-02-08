using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Website.Extensions
{
    public class SMSUserManager<TUser> : UserManager<ApplicationUser>
    {
        public SMSUserManager(IUserStore<ApplicationUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<ApplicationUser> passwordHasher, 
            IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<ApplicationUser>> logger) : 
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public override async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.RemoveFromRolesAsync(user, roles);
        }

        public override async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.AddToRolesAsync(user, roles);
        }

        public override async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if(string.IsNullOrEmpty(user?.Id))
            {
                throw new ArgumentException("User could not be empty");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.GetUserRolesAsync(user.Id);
        }

        public UserZohoContact GetUserZohoContact(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User id could not be empty");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return uStore.GetUserZohoContact(userId);
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return base.CreateAsync(user, password);
        }

        public async Task<UserZohoContact> GetUserZohoContactAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User id could not be empty");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.GetUserZohoContactAsync(userId);
            
        }

        public async Task<IdentityResult> RestPasswordAndForceChangeAsync(string userId, string password)
        {
            var user = await FindByIdAsync(userId);
            if(user == null)
            {
                return IdentityResult.Failed(new IdentityError[] {
                    new IdentityError
                    {
                        Code = "100001",
                        Description = "Could not find user"
                    }
                });
            }
            user.PasswordHash = PasswordHasher.HashPassword(user, password);
            user.LastPasswordChangedDate = user.CreationDate;
            return await UpdateAsync(user);
        }
        public async Task<bool> CheckifACLEnabled(string zohoContactUUID)
        {
            if(string.IsNullOrEmpty(zohoContactUUID))
            {
                throw new ArgumentException("zohoContactUUid could not be empty");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.CheckifACLEnabled(zohoContactUUID);
        }

        public async Task<bool> AnyCompanyExternalAdminAsync(int companyId)
        {
            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.AnyCompanyExternalAdminAsync(companyId);
        }

        public bool AnyCompanyExternalAdmin(int companyId)
        {
            var uStore = Store as SMSUserStore<ApplicationUser>;
            return uStore.AnyCompanyExternalAdmin(companyId);
        }

        public async Task<int> CreateUserZohoContactAsync(UserZohoContact userContact)
        {
            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.CreateUserZohoContactAsync(userContact);
        }


        public async Task<bool> IsInternalUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User id could not be empty");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.IsInternalUser(userId);
        }

        public override async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            if (user == null || string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(role))
            {
                throw new ArgumentException("User Id and role name is required fields");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.IsInRoleAsync(user.Id, role);
        }

        public override async Task<IdentityResult> AddPasswordAsync(ApplicationUser user, string password)
        {
            var result = await base.AddPasswordAsync(user, password);
            
            if (result.Succeeded)
            {
                var uStore = Store as SMSUserStore<ApplicationUser>;
                user.LastPasswordChangedDate = DateTime.Now;

                uStore.Context.SaveChanges();
            }

            return result;

        }
        public override async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {  
            var result =  await base.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                var uStore = Store as SMSUserStore<ApplicationUser>;
                user.LastPasswordChangedDate = DateTime.Now;
                
                uStore.Context.SaveChanges();
            }

            return result;
        }

        public Company GetCompany(int companyId)
        {
            if (companyId < 0)
            {
                throw new ArgumentException("Company Id is wrong");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return uStore.GetCompany(companyId);
        }

        public async Task<Company> GetCompanyAsync(int companyId)
        {
            if (companyId < 0)
            {
                throw new ArgumentException("Company Id is wrong");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.GetCompanyAsync(companyId);
        }

        public Company GetCompanyByZohoUuid(string zohoAccountUuid)
        {
            if (string.IsNullOrEmpty(zohoAccountUuid))
            {
                throw new ArgumentException("Zoho Account Uuid is wrong");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return uStore.GetCompanyByZohoUuid(zohoAccountUuid);
        }

        public async Task<Company> GetCompanyByZohoUuidAsync(string zohoAccountUuid)
        {
            if (string.IsNullOrEmpty(zohoAccountUuid))
            {
                throw new ArgumentException("Zoho Account Uuid is wrong");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.GetCompanyByZohoUuidAsync(zohoAccountUuid);
        }


        public IEnumerable<CompanyClaims> GetCompanyClaims(int companyId)
        {
            if(companyId < 0)
            {
                throw new ArgumentException("Company Id is wrong");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return uStore.GetCompanyClaims(companyId);
        }


        public async Task<IEnumerable<CompanyClaims>> GetCompanyClaimsAsync(int companyId)
        {
            if (companyId < 0)
            {
                throw new ArgumentException("Company Id is wrong");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.GetCompanyClaimsAsync(companyId);
        }


    }
}
