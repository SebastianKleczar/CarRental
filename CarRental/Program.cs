using CarRental.DataBase;
using CarRental.DataBase.Models;
using CarRental.Interfaces.Repositories;
using CarRental.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace CarRental
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<Client>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Context>();



            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //// Add Scopes
            builder.Services.AddTransient<ICarRepository, CarRepository>();
            builder.Services.AddTransient<IRentalRepository, RentalRepository>();
            builder.Services.AddTransient<IReviewRepository, ReviewRepository>();
            builder.Services.AddTransient<ISeed, Seed>();



            

            ////


            var app = builder.Build();



                ////

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


            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.UseStaticFiles();



           

            using (var scope = app.Services.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var userManager = (UserManager<Client>)scope.ServiceProvider.GetService(typeof(UserManager<Client>));
                var context = (Context)scope.ServiceProvider.GetService(typeof(Context));


                var AdminName = "Admin@gmail.com";
                if (context.Clients.FirstOrDefault(c => c.Email == AdminName) == null)
                {


                    context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole() { Id = "Admin", Name = "Admin", NormalizedName = "ADMIN" });
                    //this.Clients.Add(new Client() { Email ="admin", })
                    
                    var user = new Client() {UserName = "Admin@gmail.com", Email = "Admin@gmail.com" ,EmailConfirmed = true};
                    var result = userManager.CreateAsync(user, "Admin123!");
                    //var user = new Client() { Email = AdminName };
                    //var password = new PasswordHasher<Client>();
                    //var hashed = password.HashPassword(user, "Admin123!");
                    //user.PasswordHash = hashed;
                    //var addedUser = this.Clients.Add(user.Result);
                    context.SaveChanges();


                    while (!result.IsCompletedSuccessfully)
                    {
                        Thread.Sleep(1000);
                    }
                 
                }

                // do you things here


            }



            using (var scope = app.Services.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var AdminName = "Admin@gmail.com";
                var userManager = (UserManager<Client>)scope.ServiceProvider.GetService(typeof(UserManager<Client>));
                var context = (Context)scope.ServiceProvider.GetService(typeof(Context));

                var admin = context.Clients.FirstOrDefault(c => c.Email == AdminName);
                if ( admin != null && !userManager.IsInRoleAsync(admin,"Admin").Result)
                {


                    var addedUser = context.Clients.FirstOrDefault(c => c.Email == AdminName);


                    context.UserRoles.Add(new IdentityUserRole<string>() { UserId = admin.Id, RoleId = "Admin" });
                    context.SaveChanges();
                }

            }


            ///// Dodanie Admina




            app.Run();

        }
    }
}