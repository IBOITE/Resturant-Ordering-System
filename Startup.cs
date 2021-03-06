﻿using Lokanta.Data;
using Lokanta.Services;
using Lokanta.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta
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
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser,IdentityRole>(options =>
            //بستخدم الاقواس المتعرجة مشان اقدر استخدم اكتر من اوبشين
            {

                options.SignIn.RequireConfirmedAccount = false;
                //مشان زيد وقف القفل على المستخدم (الاساسي 5 دقايق) ي
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(6);
                //ععد محاولات الخطا لل المستخدم
                options.Lockout.MaxFailedAccessAttempts = 3;
            }
            
            
            )
                .AddDefaultTokenProviders()
                .AddDefaultUI()  //birinci cozum
                .AddEntityFrameworkStores<ApplicationDbContext>();
            //ikinci cozum
            services.AddSingleton<IEmailSender, EmailSender>();



            services.AddControllersWithViews();
            services.AddRazorPages();
            //shopping cart count eklemek icin
            services.AddSession(
                options => {
                    options.Cookie.IsEssential = true;
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                }
                );

            //bu online odeme icin 
            services.Configure<StripesSettings>(Configuration.GetSection("Stripe"));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];

            app.UseAuthentication();
            app.UseAuthorization();
            //shopping cart count eklemek icin
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
