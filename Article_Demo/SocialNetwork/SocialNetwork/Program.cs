using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.DbModels;

namespace SocialNetwork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<SocialNetworkDbContext>(options => options.UseSqlite("Filename=DBs/SocialNetworkDb.db"));

            //builder.WebHost.UseUrls("http://192.168.0.157:5044/");        // For local run

            var config = builder.Configuration;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SocialNetwork", builder =>
                {
                    builder.WithOrigins($"{config["Protocol"]}://{config["ArticlesDomain"]}");
                    builder.AllowCredentials();
                    builder.AllowAnyHeader();
                });
            });

            var app = builder.Build();
            app.UseCors("SocialNetwork");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Social/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "/Social/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}