using BLL.Classes;
using DAL.Data;
using DAL.Models;
using EventManagement.Models;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Configuration;

namespace EventManagement
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            //Service for Image Upload
            builder.Services.AddScoped<IFileImageService, FileImageService>();

            // Services for Stripe
            builder.Services.AddControllersWithViews();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            //services for Database

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddDbContext<ApplicationDbContext>(
                option => option.UseSqlServer(builder.Configuration.GetConnectionString("Cs")
                ));


            //services for Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                                options.SlidingExpiration = true;
                                options.AccessDeniedPath = "/System/Account/Login";
                                options.LoginPath = "/System/Account/Login";
                                options.Cookie.Expiration = null;
                            });
            //services for repository
            RepositoryDependancyInjection.AddRepositories(builder.Services);


            builder.Services.AddHangfire(config => config
                               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                               .UseSimpleAssemblyNameTypeSerializer()
                               .UseRecommendedSerializerSettings()
                               .UseSqlServerStorage(builder.Configuration.GetConnectionString("Cs"), new SqlServerStorageOptions
                               {
                                   CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                   SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                   QueuePollInterval = TimeSpan.Zero,
                                   UseRecommendedIsolationLevel = true,
                                   UsePageLocksOnDequeue = true,
                                   DisableGlobalLocks = true
                               }));


            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard();
            app.UseHangfireServer();


            app.MapAreaControllerRoute(
                name: "MyAreaEvents",
                areaName: "System",
                pattern: "System/{controller=Event}/{action=Index}/{id?}");
            app.MapAreaControllerRoute(
                name: "MyAreaOrganizers",
                areaName: "Organizer",
                pattern: "Organizer/{controller=EventOrganizer}/{action=Index}/{id?}");

            app.MapAreaControllerRoute(
                name: "MyAreaServices",
                areaName: "Admin",
                pattern: "Admin/{controller=Role}/{action=Create}/{id?}");


            app.MapControllerRoute(
                name: "MyAreas",
                pattern: "{area:exists}/{controller=Account}/{action=Login}"
                );

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=System}/{controller=Category}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
