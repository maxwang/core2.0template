using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Data;
using Website.Models;
using Website.Services;
using Website.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using PaulMiami.AspNetCore.Mvc.Recaptcha;

namespace Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<SMSUserStore<ApplicationUser>>()
                .AddUserManager<SMSUserManager<ApplicationUser>>()
                .AddSignInManager<SMSSignManager>()
                .AddRoleStore<SMSRoleStore>()
                .AddRoleManager<SMSRoleManager>()
                .AddDefaultTokenProviders();


            //extension
            services.AddScoped<SignInManager<ApplicationUser>, SMSSignManager>();

            services.AddSingleton<IAuthorizationPolicyProvider, ClaimAuthorizePolicyProvider>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, SMSClaimsPrincipalFactory>();

            //feature
            

            //for performance
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();

                options.MimeTypes = new[] { "text/plain", "application/json", "text/html" };
                //"text/plain",
                //"text/css",
                //"application/javascript",
                //"text/html",
                //"application/xml",
                //"text/xml",
                //"application/json",
                //"text/json",
            });

            // Add application services.
            services.Configure<Website.Models.SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.AddTransient<IEmailSender, EmailSender>();
            
            services.AddMvc();

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"]
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
