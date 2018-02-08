using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Website.Models;

namespace Website.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<CompanyClaims> CompanyClaims { get; set; }

        public DbSet<ApplicationRoleType> RoleTypes { get; set; }

        public DbSet<UserZohoContact> UserZohoContacts { get; set; }

        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(au =>
            {
                au.HasKey(x => x.Id);

                au.HasOne(c => c.MyCompany).WithMany(cu => cu.Users).HasForeignKey(ccu => ccu.CompanyId);

                au.HasMany(e => e.Roles)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                //au.HasMany(e => e.Claims)
                //    .WithOne()
                //    .HasForeignKey(e => e.UserId)
                //    .IsRequired()
                //    .OnDelete(DeleteBehavior.Cascade);

                //au.HasMany(e => e.Logins)
                //    .WithOne()
                //    .HasForeignKey(e => e.UserId)
                //    .IsRequired()
                //    .OnDelete(DeleteBehavior.Cascade);

                au.HasOne(uzc => uzc.ZohoContact).WithOne(zc => zc.User).HasForeignKey<UserZohoContact>(z => z.UserId);

                au.Property(c => c.CompanyId).IsRequired(false);

                au.Property(c => c.CreationDate).IsRequired(true).HasDefaultValueSql("getdate()");
                au.Property(c => c.LastPasswordChangedDate).IsRequired(true).HasDefaultValueSql("getdate()");

            });

            builder.Entity<Company>(c =>
            {
                c.HasIndex(u => u.CreatedTime).HasName("CreationTimeIndex");
                c.Property(u => u.CreatedTime).HasDefaultValueSql("getdate()");
                //c.HasMany(au => au.Users).WithOne(auc => auc.MyCompany).HasForeignKey(aucc => aucc.CompanyId).IsRequired();
                c.HasMany(au => au.Claims).WithOne(auc => auc.Company).HasForeignKey(aucc => aucc.CompanyId).IsRequired();
            });

            builder.Entity<CompanyClaims>(cc =>
            {
                cc.HasOne(ccc => ccc.Company).WithMany(cccc => cccc.Claims).HasForeignKey(aucc => aucc.CompanyId).IsRequired();
            });

            builder.Entity<Resource>(r =>
            {
                r.HasIndex(m => m.ModuleName).HasName("ix_aspnetresources_modulename");
                r.HasIndex(m => m.ClaimType).HasName("ix_aspnetresources_claimtype");
            });

            builder.Entity<ApplicationRole>(ar =>
            {
                ar.HasOne(a => a.RoleType).WithMany(at => at.Roles).HasForeignKey(atr => atr.RoleTypeId);
                ar.Property(c => c.IsInternal).IsRequired(true).HasDefaultValue(false);
            });
        }
    }
}
