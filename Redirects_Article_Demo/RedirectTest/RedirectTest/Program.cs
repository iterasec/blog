using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RedirectTest.Models.DbModels;
using RedirectTest.Services;

namespace RedirectTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.WebHost.UseUrls("http://192.168.0.157:5045/");  //For local run

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.CacheProfiles.Add("NoCache", new Microsoft.AspNetCore.Mvc.CacheProfile()
                {
                    NoStore = true
                });
            });

            builder.Services.AddDbContext<CircleScribeDbContext>(options => options.UseSqlite("Filename=DBs/RedirectTest.db"));
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "RedirectTestCookie";
                options.LoginPath = "/RedirectTest/Accounts/Login";
                options.Cookie.Path = "/RedirectTest/";
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyTestPolicy", builder =>
                {
                    builder.WithOrigins("null");
                    builder.WithMethods("POST", "GET");
                    builder.AllowCredentials();
                    builder.WithHeaders("Content-Type");
                });
            });
            builder.Services.AddTransient<StringHelper>();
            builder.Services.AddTransient<JWT_Generator>();


            var app = builder.Build();
            app.UseCors("MyTestPolicy");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseResponseCaching();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "/RedirectTest/{controller=Home}/{action=Index}"
                );

            app.Run();
        }
    }
}
